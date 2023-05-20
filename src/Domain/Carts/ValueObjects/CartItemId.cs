using Domain.Carts.Entities;

namespace Domain.Carts.ValueObjects;

public record struct CartItemId(int Value)
{
    public static CartItemId From(int cartId) => new(cartId);
}