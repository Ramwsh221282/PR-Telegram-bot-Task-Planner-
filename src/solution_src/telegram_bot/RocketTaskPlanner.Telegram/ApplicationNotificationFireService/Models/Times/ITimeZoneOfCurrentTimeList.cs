namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Times;

/// <summary>
/// Список временных зон текущего времени
/// </summary>
public interface ITimeZoneOfCurrentTimeList : IEnumerable<TimeZoneOfCurrentTime>
{
    /// <summary>
    /// Есть ли какие-либо временные зоны текущего времени
    /// </summary>
    /// <returns>True если есть, False если нет</returns>
    bool HasAnyTimeZone();
}
