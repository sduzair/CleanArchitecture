using Domain.Products.ValueObjects;

namespace Domain.Common;

public interface IAggregateRootId<TId>
{
    TId IdValue { get; protected init; }
    static abstract ProductId Create(TId value);
    static abstract ProductId CreateUnique();
}