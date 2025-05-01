using System.Collections;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Times;

public sealed class TimeZoneOfCurrentTimeList : ITimeZoneOfCurrentTimeList
{
    private readonly List<TimeZoneOfCurrentTime> _timeZones;

    public TimeZoneOfCurrentTimeList(List<TimeZoneOfCurrentTime> timeZones) =>
        _timeZones = timeZones;

    public bool HasAnyTimeZone() => _timeZones.Count != 0;

    public IEnumerator<TimeZoneOfCurrentTime> GetEnumerator() => _timeZones.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
