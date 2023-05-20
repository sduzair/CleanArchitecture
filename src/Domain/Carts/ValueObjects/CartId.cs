namespace Domain.Carts.ValueObjects;

public readonly record struct CartId(Guid Value)
{
    public static CartId Create() => new(Guid.NewGuid());
    public static CartId From(Guid value) => new(value);
    //create from string
    public static CartId From(string value) => new(Guid.Parse(value));
}