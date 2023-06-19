using Domain.Common;

namespace Presentation.Carts;

public class CartItemDto : ValueObject
{
    public Guid ProductId { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public decimal UnitPrice { get; init; }
    public int Quantity { get; set; }

    public CartItemDto(Guid productId, string name, string description, decimal unitPrice, int quantity)
    {
        ProductId = productId;
        Name = name;
        Description = description;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public void UpdateQuantity(int quantity)
    {
        Quantity += quantity;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ProductId;
        yield return Name;
        yield return Description;
        yield return UnitPrice;
    }
}
