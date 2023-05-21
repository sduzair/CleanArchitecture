using System.Security.Claims;

using Application.Carts.Commands;
using Application.Carts.Queries;
using Application.Common.Security.Roles;
using Application.Customers.Queries;

using Domain.Carts.ValueObjects;
using Domain.Customers.ValueObjects;

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
[Authorize]
public sealed class CartController : ApiControllerBase
{
    private readonly ApplicationAspNetCoreResultEndpointProfile _resultProfile;

    public CartController(ApplicationAspNetCoreResultEndpointProfile resultProfile)
    {
        _resultProfile = resultProfile;
    }

    [HttpGet]
    public async Task<IActionResult> GetCartAsync()
    {
        var cart = GetCartFromSession() ?? CartDto.Create();

        if (User.IsInRole(nameof(Customer)))
        {
            if (cart.IsNotPersistedAndHasItems)
            {
                await Mediator.Send(new AddItemsToCartCommand(await GetCartIdFromSessionAsync(), cart.Items.Select(product => product.MapTo()).ToList()));
                cart = cart.AsPersisted();
            }

            var dbCart = (await Mediator.Send(new GetCartQuery(await GetCartIdFromSessionAsync()))).Value;
            cart = cart.MapWith(dbCart);
        }

        SetCartInSession(cart);

        return Ok(cart);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateItemQuantityOrAddItem(CartItemDto cartItem)
    {
        if (User.IsInRole(nameof(Customer)))
        {
            var result = await Mediator.Send(new AddItemOrUpdateQuantityCommand(await GetCartIdFromSessionAsync(), cartItem.MapTo()));

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
    public async Task<IActionResult> RemoveCartItemAsync(CartItemDto cartItem)
    {
        if (User.IsInRole(nameof(Customer)))
        {
            await Mediator.Send(new RemoveCartItemCommand(await GetCartIdFromSessionAsync(), cartItem.MapTo()));

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

    private async Task<CartId> GetCartIdFromSessionAsync()
    {
        string? cartId = HttpContext.Session.GetString(SessionKeys.CartId);

        if (cartId != null) return CartId.From(cartId);

        var (_, cartIdFromDb) = await GetCustomerIncludeCartAndCreateCartIfNotExistsAndUpdateSession();
        return cartIdFromDb;
    }

    private async Task<CustomerId> GetCustomerIdFromSessionAsync()
    {
        var customerId = HttpContext.Session.GetString(SessionKeys.CustomerId);

        if (customerId != null) return CustomerId.From(customerId);
        var (customerIdFromDb, _) = await GetCustomerIncludeCartAndCreateCartIfNotExistsAndUpdateSession();
        return customerIdFromDb;
    }

    private async Task<(CustomerId, CartId)> GetCustomerIncludeCartAndCreateCartIfNotExistsAndUpdateSession()
    {
        var applicationUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await Mediator.Send(new GetCustomerByApplicationUserIdIncludeCartQuery(applicationUserId));

        CartId cartId;
        if(result.Value.Cart is null)
        {
            var newCartId = await Mediator.Send(new CreateCartCommand(result.Value.Id));
            cartId = newCartId.Value;
        }
        else
        {
            cartId = result.Value.Cart!.Id;
        }

        HttpContext.Session.SetString(SessionKeys.CartId, cartId.Value.ToString());

        CustomerId customerId = result.Value.Id;
        HttpContext.Session.SetString(SessionKeys.CustomerId, customerId.Value.ToString());

        return (customerId, cartId);
    }
}
