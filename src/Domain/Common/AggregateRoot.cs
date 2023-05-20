namespace Domain.Common;

public abstract class AggregateRoot<TId> : Entity<TId> where TId : struct
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