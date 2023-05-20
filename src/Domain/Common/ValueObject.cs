namespace Domain.Common;

public abstract class ValueObject : IValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();

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
        if (obj is not ValueObject other) return false;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public bool Equals(IValueObject? other)
    {
        return Equals(other);
    }

    public static bool operator ==(ValueObject a, ValueObject b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(ValueObject a, ValueObject b)
    {
        return !(a == b);
    }

    public int CompareTo(object? obj)
    {
        if (obj == null)
            return 1;

        var thisType = GetType();
        var otherType = obj.GetType();

        if (thisType != otherType)
            return string.Compare(thisType.ToString(), otherType.ToString(), StringComparison.Ordinal);

        var other = (ValueObject)obj;

        var components = GetEqualityComponents().ToArray();
        var otherComponents = other.GetEqualityComponents().ToArray();

        for (var i = 0; i < components.Length; i++)
        {
            var comparison = CompareComponents(components[i], otherComponents[i]);
            if (comparison != 0)
                return comparison;
        }

        return 0;
    }
    private static int CompareComponents(object? object1, object? object2)
    {
        if (object1 is null && object2 is null)
            return 0;

        if (object1 is null)
            return -1;

        if (object2 is null)
            return 1;

        if (object1 is IComparable comparable1 && object2 is IComparable comparable2)
            return comparable1.CompareTo(comparable2);

        return object1.Equals(object2) ? 0 : -1;
    }

    public int CompareTo(IValueObject? other)
    {
        return CompareTo(other as object);
    }
}
