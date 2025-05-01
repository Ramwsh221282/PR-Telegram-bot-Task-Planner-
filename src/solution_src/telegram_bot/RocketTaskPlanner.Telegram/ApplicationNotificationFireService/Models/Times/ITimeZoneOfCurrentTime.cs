using RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Receivers;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Times;

public interface ITimeZoneOfCurrentTime
{
    public string ZoneName();
    public DateTime DateTime();
    public string Id();
    Task<GeneralChatReceiverOfCurrentTimeZone[]> Receivers();
    ITimeZoneOfCurrentTime WithReceivers(GeneralChatReceiverOfCurrentTimeZone[] receivers);
}
