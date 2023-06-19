using System.Security.Claims;

using Application.Carts.Commands;
using Application.Carts.Queries;
using Application.Common.Security.Policies;
using Application.Customers.Queries;

using Domain.Carts.ValueObjects;

using FluentResults.Extensions.AspNetCore;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Persistence.Identity.Roles;

using Presentation.Common;
using Presentation.Common.Extensions;

namespace Presentation.Carts;

/// <summary>
/// If the user is signed in as <see cref="Customer"/> and cart in session has items that haven't been persisted the items are persisted. The Session is cleared on logout.
/// </summary>
public sealed partial class CartController : ApiControllerBase
{
    private readonly CustomAspNetCoreResultEndpointProfile _resultProfile;
    private readonly SessionCart _sessionCart;

    public CartController(CustomAspNetCoreResultEndpointProfile resultProfile, IHttpContextAccessor httpContextAccessor)
    {
        _resultProfile = resultProfile;
        _sessionCart = new SessionCart(httpContextAccessor.HttpContext!.Session);
    }

    [HttpGet]
    [Authorize(Policy = nameof(CartPolicy))]
    public async Task<IActionResult> GetCartAsync()
    {
        if (User.IsInRole(nameof(Customer)))
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cartId = _sessionCart.GetCartId() ?? await GetCartIdCreateCartIfNotExistsAndAddCartIdToSessionAsync(userId);

            if (_sessionCart.GetCart().IsNotPersistedAndHasItems)
            {
                await Mediator.Send(new AddItemsToCartCommand(cartId, _sessionCart.GetCart().Items
                    .Select(item => item.MapTo())
                    .ToList()));
                _sessionCart.SetAsPersisted();
            }

            var dbCart = (await Mediator.Send(new GetCartQuery(cartId))).Value;

            return Ok(dbCart.MapToDto());
        }

        return Ok(_sessionCart.GetCart());
    }

    [HttpPost]
    [Authorize(Policy = nameof(CartPolicy))]
    public async Task<IActionResult> AddItemOrUpdateItemQuantity(CartItemDto cartItemDto)
    {
        if (User.IsInRole(nameof(Customer)))
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cartId = _sessionCart.GetCartId() ?? await GetCartIdCreateCartIfNotExistsAndAddCartIdToSessionAsync(userId);

            var result = await Mediator.Send(new AddItemOrUpdateQuantityCommand(cartId, cartItemDto.MapTo()));

            if (result.IsFailed)
            {
                return result.ToActionResult(_resultProfile);
            }

            return NoContent();
        }

        _sessionCart.AddItemOrUpdateQuantity(cartItemDto);

        return NoContent();
    }

    [HttpPost]
    [Authorize(Policy = nameof(CartPolicy))]
    public async Task<IActionResult> RemoveCartItemAsync(CartItemDto cartItemDto)
    {
        if (User.IsInRole(nameof(Customer)))
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cartId = _sessionCart.GetCartId() ?? await GetCartIdCreateCartIfNotExistsAndAddCartIdToSessionAsync(userId);

            var result = await Mediator.Send(new RemoveCartItemCommand(cartId, cartItemDto.MapTo()));

            if (result.IsFailed)
            {
                return result.ToActionResult(_resultProfile);
            }

            return NoContent();
        }

        _sessionCart.RemoveItem(cartItemDto);

        return NoContent();
    }

    private async Task<CartId> GetCartIdCreateCartIfNotExistsAndAddCartIdToSessionAsync(Guid userId)
    {
        var customer = (await Mediator.Send(new GetCustomerByUserIdIncludeCartQuery(userId))).Value;
        CartId cartId;
        if (HasNoCart(customer))
        {
            cartId = (await Mediator.Send(new CreateCartCommand(customer.Id))).Value;
        }
        else { cartId = customer.Cart!.Id; }

        _sessionCart.SetCartId(cartId);

        return cartId;

        static bool HasNoCart(Domain.Customers.Customer customer)
        {
            return customer.Cart is null;
        }
    }
    /// <summary>
    /// To track the items added to the cart (not persisted) in session by the user when the user is a <see cref="Visitor"/> 
    /// </summary>
    private class SessionCart
    {
        private readonly ISession _session;

        public SessionCart(ISession session)
        {
            _session = session;
        }

        private static class SessionKeys
        {
            public const string Cart = "Cart";
            public const string CartId = "CartId";
            public const string CustomerId = "CustomerId";
        }

        private CartDto? _sessionCart;

        public CartDto GetCart()
        {
            if (_sessionCart != null)
            {
                return _sessionCart;
            }

            var sessionCart = _session.GetObject<CartDto>(SessionKeys.Cart);
            if (sessionCart == null)
            {
                sessionCart = CartDto.Create();
                _session.SetObject(SessionKeys.Cart, sessionCart);
            }
            _sessionCart = sessionCart;

            return _sessionCart;
        }

        internal void AddItemOrUpdateQuantity(CartItemDto cartItem)
        {
            if (_sessionCart == null)
            {
                _sessionCart = GetCart();
            }

            var updatedCart = _sessionCart.AddItemOrUpdateQuantity(cartItem);
            _session.SetObject(SessionKeys.Cart, updatedCart);
            _sessionCart = updatedCart;
        }

        internal void RemoveItem(CartItemDto cartItemDto)
        {
            if (_sessionCart == null)
            {
                _sessionCart = GetCart();
            }

            var updatedCart = _sessionCart.RemoveItem(cartItemDto);
            _session.SetObject(SessionKeys.Cart, updatedCart);
            _sessionCart = updatedCart;
        }

        internal void SetAsPersisted()
        {
            if (_sessionCart == null)
            {
                _sessionCart = GetCart();
            }

            var updatedCart = _sessionCart.AsPersisted();
            _session.SetObject(SessionKeys.Cart, updatedCart);
            _sessionCart = updatedCart;
        }

        public CartId? GetCartId()
        {
            string? cartId = _session.GetString(SessionKeys.CartId);

            if (cartId != null) return CartId.From(cartId);
            else return null;
        }

        public void SetCartId(CartId cartId)
        {
            _session.SetString(SessionKeys.CartId, cartId.Value.ToString());
        }
    }
}

