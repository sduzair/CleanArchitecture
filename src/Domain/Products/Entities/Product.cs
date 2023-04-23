using Domain.Common;
using Domain.Products.ValueObjects;

namespace Domain.Products.Entities;
public class Product : AggregateRoot<ProductId, Guid>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    protected Product(string name, string description, decimal price)
    {
        Id = ProductId.CreateUnique();
        Name = name;
        Description = description;
        Price = price;
    }
    public static Product Create(string name, string description, decimal price)
    {
        return new (name, description, price);
    }

    public void Update(string name, string description, decimal unitPrice)
    {
        Name = name;
        Description = description;
        Price = unitPrice;
    }
}
