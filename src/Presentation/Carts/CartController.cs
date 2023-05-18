using System.Security.Claims;

using Application.Carts.Commands;
using Application.Carts.Queries;
using Application.Common.Security.Roles;
using Application.Customers.Queries;

using Domain.Carts.ValueObjects;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Presentation.Common;
using Presentation.Common.Extensions;
using Presentation.Contracts.Carts;

namespace Presentation.Carts;

[Authorize]
public sealed class CartController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCartAsync()
    {
        //Does Cart exist in Session? If not, get it create one.
        var cart = GetCartFromSession() ?? CartDto.Create();

        //Is User logged in as Customer?
        if (User.IsInRole(nameof(Customer)))
        {
            //Is Session Cart not persisted?
            if (cart.IsNotPersistedAndHasItems)
            {
                //Update Cart in Store
                await Mediator.Send(new AddCartItemsCommand(await GetCartIdFromSessionAsync(), cart.Items.Select(product => product.MapTo()).ToHashSet()));
                cart = cart.AsPersisted();
            }

            //Get Cart from Store
            var dbCart = (await Mediator.Send(new GetCartQuery())).Value;
            cart = cart.MapWith(dbCart);
        }

        SetCartInSession(cart);

        return Ok(cart);
    }

    [HttpPost]
    public async Task<IActionResult> SetCartItemQuatityAsync(CartItemDto cartItem)
    {
        if (User.IsInRole(nameof(Customer)))
        {
            await Mediator.Send(new SetCartItemQuantityCommand(await GetCartIdFromSessionAsync(), cartItem.MapTo()));
            return NoContent();
        }

        CartDto cart = GetCartFromSession() ?? CartDto.Create();

        cart = cart.SetItem(cartItem);
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

        cart = cart.RemoveItem(cartItem);
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
        //remove cart key
        //HttpContext.Session.Remove(SessionKeys.Cart);
        HttpContext.Session.SetObject(SessionKeys.Cart, cart);
    }

    private async Task<CartId> GetCartIdFromSessionAsync()
    {
        var cartId = HttpContext.Session.GetString(SessionKeys.CartId);
        if (cartId == null)
        {
            var applicationUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await Mediator.Send(new GetCustomerQuery(applicationUserId));
            cartId = result.Value.Cart!.Id.Value.ToString();
            HttpContext.Session.SetString(SessionKeys.CartId, cartId);
        }
        return CartId.From(cartId);
    }

}