namespace Application.Interfaces;
public interface IDateTimeProvider
{
    public DateTimeOffset UtcNow { get; }
}
