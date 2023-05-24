using System.Security.Claims;

using Application.Carts.Commands;
using Application.Carts.Queries;
using Application.Common.Security.Policies;
using Application.Common.Security.Roles;
using Application.Customers.Queries;

using Domain.Carts.ValueObjects;
using Domain.Customers.ValueObjects;

using FluentResults;
using FluentResults.Extensions.AspNetCore;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Presentation.Common;
using Presentation.Common.Extensions;
using Presentation.Contracts.Carts;

namespace Presentation.Carts;

/// <summary>
/// The purpose of the Session is to track the items added to the cart by the user when the user is not logged in. The Session is cleared on logout.
/// </summary>
public sealed class CartController : ApiControllerBase
{
    private readonly ApplicationAspNetCoreResultEndpointProfile _resultProfile;

    public CartController(ApplicationAspNetCoreResultEndpointProfile resultProfile)
    {
        _resultProfile = resultProfile;
    }

    [HttpGet]
    [Authorize(Policy = nameof(CartPolicy))]
    public async Task<IActionResult> GetCartAsync()
    {
        var cart = GetCartFromSession() ?? CartDto.Create();

        if (User.IsInRole(nameof(Customer)))
        {
            var cartId = GetCartIdFromSession() ?? (await GetCustomerIncludingCartAndAddIdsToSessionAsync()).Cart!.Id;

            if (cart.IsNotPersistedAndHasItems)
            {
                await Mediator.Send(new AddItemsToCartCommand(cartId, cart.Items.Select(product => product.MapTo()).ToList()));
                cart = cart.AsPersisted();
            }

            var dbCart = (await Mediator.Send(new GetCartQuery(cartId))).Value;
            cart = cart.MapWith(dbCart);
        }

        SetCartInSession(cart);

        return Ok(cart);
    }

    [HttpPost]
    [Authorize(Policy = nameof(CartPolicy))]
    public async Task<IActionResult> UpdateItemQuantityOrAddItem(CartItemDto cartItem)
    {
        if (User.IsInRole(nameof(Customer)))
        {
            var cartId = GetCartIdFromSession() ?? (await GetCustomerIncludingCartAndAddIdsToSessionAsync()).Cart!.Id;

            var result = await Mediator.Send(new AddItemOrUpdateQuantityCommand(cartId, cartItem.MapTo()));

            if (result.IsFailed)
            {
                return result.ToActionResult(_resultProfile);
            }

            ///returning here is necessary because otherwise <see cref="CartDto.IsNotPersistedAndHasItems"/> will become true despite the items being already persisted
            return NoContent();
        }

        CartDto cart = GetCartFromSession() ?? CartDto.Create();

        cart = cart.SetItem(cartItem)
            .AsNotPersisted();

        SetCartInSession(cart);

        return NoContent();
    }

    [HttpPost]
    [Authorize(Policy = nameof(CartPolicy))]
    public async Task<IActionResult> RemoveCartItemAsync(CartItemDto cartItem)
    {
        if (User.IsInRole(nameof(Customer)))
        {
            var cartId = GetCartIdFromSession() ?? (await GetCustomerIncludingCartAndAddIdsToSessionAsync()).Cart!.Id;

            await Mediator.Send(new RemoveCartItemCommand(cartId, cartItem.MapTo()));

            return NoContent();
        }

        var cart = GetCartFromSession() ?? CartDto.Create();

        cart = cart.RemoveItem(cartItem)
            .AsNotPersisted();

        SetCartInSession(cart);

        return NoContent();
    }


    private static class SessionKeys
    {
        public const string Cart = "Cart";
        public const string CartId = "CartId";
        public const string CustomerId = "CustomerId";
    }

    private CartDto? GetCartFromSession()
    {
        CartDto? cartDto = HttpContext.Session.GetObject<CartDto>(SessionKeys.Cart);
        return cartDto;
    }

    private void SetCartInSession(CartDto cart)
    {
        HttpContext.Session.SetObject(SessionKeys.Cart, cart);
    }

    private CartId? GetCartIdFromSession()
    {
        string? cartId = HttpContext.Session.GetString(SessionKeys.CartId);

        if (cartId != null) return CartId.From(cartId);
        else return null;
    }

    private CustomerId? GetCustomerIdFromSession()
    {
        var customerId = HttpContext.Session.GetString(SessionKeys.CustomerId);

        if (customerId != null) return CustomerId.From(customerId);
        else return null;
    }

    private async Task<Domain.Customers.Customer> GetCustomerIncludingCartAndAddIdsToSessionAsync()
    {
        Result<Domain.Customers.Customer> result = await GetCustomerIncludeCartAsync();

        CartId cartId;
        if (result.Value.Cart is null)
        {
            var newCartId = await Mediator.Send(new CreateCartCommand(result.Value.Id));
            cartId = newCartId.Value;
        }
        else
        {
            cartId = result.Value.Cart!.Id;
        }
        AddCartIdToSession(cartId);

        CustomerId customerId = result.Value.Id;
        AddCustomerIdToSession(customerId);

        return result.Value;

        async Task<Result<Domain.Customers.Customer>> GetCustomerIncludeCartAsync()
        {
            var applicationUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await Mediator.Send(new GetCustomerByApplicationUserIdIncludeCartQuery(applicationUserId));
            return result;
        }

        void AddCustomerIdToSession(CustomerId customerId)
        {
            HttpContext.Session.SetString(SessionKeys.CustomerId, customerId.Value.ToString());
        }

        void AddCartIdToSession(CartId cartId)
        {
            HttpContext.Session.SetString(SessionKeys.CartId, cartId.Value.ToString());
        }
    }

}
