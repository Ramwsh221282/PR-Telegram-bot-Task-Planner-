using RocketTaskPlanner.Utilities.UnixTimeUtilities;

namespace RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones.ValueObjects;

/// <summary>
/// Дата временной зоны
/// </summary>
public readonly record struct TimeZoneTimeInfo
{
    /// <summary>
    /// Дата в Unix секундах
    /// </summary>
    public long TimeStamp { get; }

    /// <summary>
    /// Дата и время в формате <inheritdoc cref="DateTime"/>
    /// </summary>
    public DateTime DateTime { get; }

    public TimeZoneTimeInfo()
    {
        TimeStamp = -1;
        DateTime = DateTime.MinValue;
    }

    public TimeZoneTimeInfo(long timeStamp)
    {
        TimeStamp = timeStamp;
        DateTime = timeStamp.FromUnixTimeSeconds();
    }

    public static Result<TimeZoneTimeInfo> Create(long timeStamp) =>
        timeStamp <= 0
            ? Result.Failure<TimeZoneTimeInfo>("Некорректный time stamp")
            : new TimeZoneTimeInfo(timeStamp);

    public string GetDateString() => DateTime.ToString("HH:mm:ss dd/MM/yyyy");
}
