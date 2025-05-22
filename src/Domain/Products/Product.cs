using Domain.Carts.Entities;
using Domain.Common;
using Domain.Products.ValueObjects;

using FluentResults;

namespace Domain.Products;

public sealed class Product : AggregateRoot<ProductId>
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }

    //ef core constructor to map entity
    private Product() { }

    private Product(ProductId id, string name, string description, decimal price, int stock)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
    }

    public static Result<Product> Create(ProductId id, string name, string description, decimal price, int stock)
    {
        //TODO: add validation
        return new Product
        (
            id: id,
            name: name,
            description: description,
            price: price,
            stock: stock
        );
    }

    public void Update(string name, string description, decimal unitPrice)
    {
        Name = name;
        Description = description;
        Price = unitPrice;
    }

    public void AddStock(int quantity)
    {
        Stock += quantity;
    }
}
