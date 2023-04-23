using Domain.Common;

namespace Domain.Products.ValueObjects;
public record ProductId(Guid IdValue) : IAggregateRootId<Guid>
{
    public static ProductId CreateUnique() => new(Guid.NewGuid());
    public static ProductId Create(Guid value) => new(value);
}
