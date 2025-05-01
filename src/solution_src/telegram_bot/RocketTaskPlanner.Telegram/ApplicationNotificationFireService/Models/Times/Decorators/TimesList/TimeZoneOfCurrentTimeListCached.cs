using System.Collections;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Cache;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Times.Decorators.TimesList;

public sealed class TimeZoneOfCurrentTimeListCached : ITimeZoneOfCurrentTimeList
{
    private readonly ITimeZoneOfCurrentTimeList _list;

    public TimeZoneOfCurrentTimeListCached(
        ITimeZoneOfCurrentTimeList list,
        TimeZoneDbProviderCachedInstance instance
    )
    {
        _list = list;
        if (instance.Instance == null)
            return;
        List<TimeZoneOfCurrentTime> times =
        [
            .. instance.Instance.TimeZones.Select(t => new TimeZoneOfCurrentTime(t)),
        ];
        _list = new TimeZoneOfCurrentTimeList(times);
    }

    public bool HasAnyTimeZone() => _list.HasAnyTimeZone();

    public IEnumerator<TimeZoneOfCurrentTime> GetEnumerator() => _list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
