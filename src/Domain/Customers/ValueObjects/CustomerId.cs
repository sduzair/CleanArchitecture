namespace Domain.Customers.ValueObjects;

public readonly record struct CustomerId(Guid Value)
{
    public static CustomerId Create() => new(Guid.NewGuid());
    public static CustomerId From(Guid value) => new(value);
    public static CustomerId From(string value) => new(Guid.Parse(value));
}