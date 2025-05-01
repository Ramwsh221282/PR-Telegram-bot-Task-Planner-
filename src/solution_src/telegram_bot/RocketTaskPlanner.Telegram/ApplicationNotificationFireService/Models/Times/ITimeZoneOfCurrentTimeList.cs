namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Times;

public interface ITimeZoneOfCurrentTimeList : IEnumerable<TimeZoneOfCurrentTime>
{
    bool HasAnyTimeZone();
}
