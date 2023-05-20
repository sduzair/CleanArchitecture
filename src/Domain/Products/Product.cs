using Domain.Common;
using Domain.Products.ValueObjects;

namespace Domain.Products;

public sealed class Product : AggregateRoot<ProductId>
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }

    //ef core constructor to map entity
    private Product() { }

    public static Product Create(string name, string description, decimal price, int stock)
    {
        return new()
        {
            Id = ProductId.Create(),
            Name = name,
            Description = description,
            Price = price,
            Stock = stock,
        };
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
