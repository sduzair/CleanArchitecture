namespace Domain.Products.ValueObjects;

public readonly record struct ProductId(Guid Value)
{
    public static ProductId Create() => new(Guid.NewGuid());
    public static ProductId From(Guid value) => new(value);
    public static ProductId From(string value) => new(Guid.Parse(value));
}
