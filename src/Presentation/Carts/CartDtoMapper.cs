using Domain.Carts;
using Domain.Carts.Entities;
using Domain.Products.ValueObjects;

namespace Presentation.Carts;

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

    public static CartItemDto MapToDto(this CartItem cartItem)
    {
        return new CartItemDto(cartItem.ProductId.Value, cartItem.Name, cartItem.Description, cartItem.UnitPrice, cartItem.Quantity);
    }

    public static CartDto MapToDto(this Cart cart)
    {
        return new CartDto(
            cart.Id.Value,
            cart.Items.Select(item => item.MapToDto()).ToHashSet(),
            cart.Items.Aggregate(0, (acc, item) => acc + item.Quantity),
            cart.Items.Aggregate(0m, (acc, item) => acc + item.UnitPrice));
    }
}
