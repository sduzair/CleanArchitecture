using Domain.Carts.Errors;
using Domain.Common;
using Domain.Products;
using Domain.Products.ValueObjects;

using FluentResults;

namespace Domain.Carts.Entities;

public sealed class CartItem : ValueObject
{
    public string Name { get; init; }
    public string Description { get; init; }
    public decimal UnitPrice { get; init; }
    public int Quantity { get; private set; }

    //FK
    public ProductId ProductId { get; init; }

    //TODO - check if product can be modified through item 
    //NAVIGATION
    public Product? Product { get; private set; } = null!;

    //ef core constructor to map entity
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private CartItem() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private CartItem(ProductId productId, string name, string description, decimal unitPrice, int quantity = 1)
    {
        Name = name;
        Description = description;
        UnitPrice = unitPrice;
        Quantity = quantity;
        ProductId = productId;
    }

    public static CartItem Create(ProductId productId, string name, string description, decimal unitPrice, int quantity = 1)
    {
        return new CartItem(productId, name, description, unitPrice, quantity);
    }

    //public static CartItem From(CartItemId cartItemId, CartId cartId, Product product)
    //{
    //    return new CartItem()
    //    {
    //        Id = cartItemId,
    //        CartId = cartId,
    //        ProductId = product.Id,
    //        Name = product.Name,
    //        UnitPrice = product.Price,
    //        Quantity = 1
    //    };
    //}

    public Result UpdateQuantity(int quantity)
    {
        var result = new Result();
        if(!MustNotBeZeroQuantity(Quantity, quantity))
        {
            result.WithError(new MustNotBeZeroQuantityValidationError());
        }
        if (!MustHavePositiveQuantity(Quantity, quantity))
        {
            result.WithError(new MustHavePositiveTotalQuantityValidationError(Quantity + quantity));
        }
        if (result.IsFailed)
        {
            return result;
        }

        Quantity += quantity;
        return result;
    }

    public static bool MustNotBeZeroQuantity(int oldQuantity, int newQuantity)
    {
        return oldQuantity + newQuantity != 0;
    }

    public static bool MustHavePositiveQuantity(int oldQuantity, int newQuantity)
    {
        return oldQuantity + newQuantity > 0;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return Description;
        yield return UnitPrice;
        yield return ProductId;
    }
}
