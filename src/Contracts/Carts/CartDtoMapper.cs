using Domain.Carts.Entities;
using Domain.Products.ValueObjects;

namespace Presentation.Contracts.Carts;

public static class CartDtoMapper
{
    public static CartItem MapTo(this CartItemDto cartItemDto)
    {
        return CartItem.Create(ProductId.From(cartItemDto.ProductId),
            cartItemDto.Name,
            cartItemDto.Description,
            cartItemDto.UnitPrice,
            cartItemDto.Quantity);
    }

    public static CartItemDto MapTo(this CartItem cartItem)
    {
        return new CartItemDto(cartItem.ProductId.Value, cartItem.Name, cartItem.Description, cartItem.UnitPrice, cartItem.Quantity);
    }

}
