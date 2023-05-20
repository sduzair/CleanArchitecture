namespace Domain.Common;

public interface IEntity<TId> : IEquatable<IEntity<TId>> where TId : struct
{
    TId Id { get; init; }
    bool Equals(object? obj);
    int GetHashCode();
}
