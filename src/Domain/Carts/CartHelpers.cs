using Domain.Carts.Entities;
using Domain.Products;

namespace Domain.Carts;

public static class CartHelpers
{
    public static CartItem MapToCartItem(this Product product, int quantity)
    {
        return CartItem.Create(product.Id, product.Name, product.Description, product.Price, quantity);
    }
}