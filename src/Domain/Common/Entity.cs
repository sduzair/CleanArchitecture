namespace Domain.Common;

public abstract class Entity<TId> : IEntity<TId> where TId : struct
{
    public TId Id { get; init; }

    protected virtual IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
    }

    private int? _cachedHashCode;
    public override int GetHashCode()
    {
        if (!_cachedHashCode.HasValue)
        {
            _cachedHashCode = GetEqualityComponents()
                .Aggregate(1, (current, next) =>
                {
                    unchecked
                    {
                        return (current * 23) + next?.GetHashCode() ?? 0;
                    }
                });
        }

        return _cachedHashCode.Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
            return false;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public bool Equals(IEntity<TId>? other)
    {
        return Equals(other);
    }

    public static bool operator ==(Entity<TId>? entity, Entity<TId>? other)
    {
        if (entity is null && other is null)
            return true;

        if (entity is null || other is null)
            return false;

        return entity.Equals(other);
    }

    public static bool operator !=(Entity<TId>? entity, Entity<TId>? other)
    {
        return !(entity == other);
    }
}
