namespace RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

public sealed record class NotificationReceiverTimeZone
{
    public string ZoneName { get; }
    public long TimeStamp { get; }

    private NotificationReceiverTimeZone()
    {
        ZoneName = string.Empty;
        TimeStamp = 0;
    } // ef core

    private NotificationReceiverTimeZone(string zoneName, long timeStamp)
    {
        ZoneName = zoneName;
        TimeStamp = timeStamp;
    }

    public static Result<NotificationReceiverTimeZone> Create(string? zoneName, long timeStamp)
    {
        if (string.IsNullOrWhiteSpace(zoneName))
            return Result.Failure<NotificationReceiverTimeZone>("Временная зона была пустой");

        if (timeStamp <= 0)
            return Result.Failure<NotificationReceiverTimeZone>(
                "Секунды временной зоны некорректы"
            );

        return new NotificationReceiverTimeZone(zoneName, timeStamp);
    }
}
