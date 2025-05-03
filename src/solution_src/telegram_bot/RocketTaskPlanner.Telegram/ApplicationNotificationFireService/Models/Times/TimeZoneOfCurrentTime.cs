using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Receivers;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Times;

/// <summary>
/// Временная зона текущего времени
/// </summary>
public sealed class TimeZoneOfCurrentTime : ITimeZoneOfCurrentTime
{
    private readonly ApplicationTimeZone _timeZone;
    private GeneralChatReceiverOfCurrentTimeZone[] _receivers = [];

    public TimeZoneOfCurrentTime(ApplicationTimeZone timeZone) => _timeZone = timeZone;

    public string ZoneName() => _timeZone.Name.Name;

    public DateTime DateTime() => _timeZone.TimeInfo.DateTime;

    public string Id() => _timeZone.Id.Id;

    public async Task<GeneralChatReceiverOfCurrentTimeZone[]> Receivers() =>
        await Task.FromResult(_receivers);

    public ITimeZoneOfCurrentTime WithReceivers(GeneralChatReceiverOfCurrentTimeZone[] receivers) =>
        new TimeZoneOfCurrentTime(_timeZone) { _receivers = receivers };
}
