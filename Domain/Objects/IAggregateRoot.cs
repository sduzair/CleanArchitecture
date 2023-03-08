namespace Domain.Objects;

public interface IAggregateRoot
{
    Guid Id { get; init; }
}