namespace Domain.Common;

public abstract class AggregateRoot<TId, TValue> : Entity<TId> where TId : IAggregateRootId<TValue>
{
    //private readonly List<DomainEventBase> _domainEvents = new();

    //[NotMapped]
    //public IReadOnlyCollection<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

    //protected void AddDomainEvent(DomainEventBase domainEvent)
    //{
    //    _domainEvents.Add(domainEvent);
    //}

    //internal void ClearDomainEvents()
    //{
    //    _domainEvents.Clear();
    //}
}