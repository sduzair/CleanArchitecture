namespace Domain.Products.ValueObjects;

public record ProductId(Guid Value)
{
    public static ProductId CreateUnique() => new(Guid.NewGuid());
    public static ProductId Create(Guid value) => new(value);
    //create from string
    public static ProductId Create(string value) => new(Guid.Parse(value));

}
