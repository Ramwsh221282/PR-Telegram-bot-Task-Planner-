using RocketTaskPlanner.Utilities.UnixTimeUtilities;

namespace RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones.ValueObjects;

public readonly record struct TimeZoneTimeInfo
{
    public long TimeStamp { get; }
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
