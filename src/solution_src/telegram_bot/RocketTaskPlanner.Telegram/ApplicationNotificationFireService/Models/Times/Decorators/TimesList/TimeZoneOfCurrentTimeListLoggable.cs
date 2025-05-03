using System.Collections;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Times.Decorators.TimesList;

/// <summary>
/// Список временных зон текущего времени
/// </summary>
public sealed class TimeZoneOfCurrentTimeListLoggable : ITimeZoneOfCurrentTimeList
{
    private readonly Serilog.ILogger _logger;
    private readonly ITimeZoneOfCurrentTimeList _list;
    private const string CONTEXT = nameof(ITimeZoneOfCurrentTimeList);

    public TimeZoneOfCurrentTimeListLoggable(
        ITimeZoneOfCurrentTimeList list,
        Serilog.ILogger logger
    )
    {
        _list = list;
        _logger = logger;
    }

    public bool HasAnyTimeZone()
    {
        bool hasAnyTimeZone = _list.HasAnyTimeZone();
        _logger.Information("{Context}. Has any time zone: {Has}", CONTEXT, hasAnyTimeZone);
        return hasAnyTimeZone;
    }

    public IEnumerator<TimeZoneOfCurrentTime> GetEnumerator() => _list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
