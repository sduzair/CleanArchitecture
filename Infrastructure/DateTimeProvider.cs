using Application.Interfaces;

namespace Infrastructure;

internal sealed class UtcClock : ITimeProvider
{
    /// <summary>
    /// Returns the current UTC (date)time.
    /// </summary>
    /// <returns>Returns the current UTC (date)time.</returns>
    public DateTimeOffset GetTime() => DateTimeOffset.UtcNow;
}
