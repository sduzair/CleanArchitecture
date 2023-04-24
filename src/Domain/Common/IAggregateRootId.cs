using Domain.Products.ValueObjects;

namespace Domain.Common;

public interface IAggregateRootId<TId>
{
    TId Value { get; protected init; }
    static abstract ProductId Create(TId value);
    static abstract ProductId CreateUnique();
}