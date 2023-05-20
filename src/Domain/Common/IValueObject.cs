namespace Domain.Common;

public interface IValueObject : IComparable, IComparable<IValueObject>, IEquatable<IValueObject>
{
    bool Equals(object? obj);
    int GetHashCode();
}
