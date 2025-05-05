namespace RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

public sealed record class NotificationReceiverTimeZone
{
    public string ZoneName { get; }

    private NotificationReceiverTimeZone() => ZoneName = string.Empty; // ef core

    private NotificationReceiverTimeZone(string zoneName) => ZoneName = zoneName;

    public static Result<NotificationReceiverTimeZone> Create(string? zoneName)
    {
        return string.IsNullOrWhiteSpace(zoneName)
            ? Result.Failure<NotificationReceiverTimeZone>("Временная зона была пустой")
            : new NotificationReceiverTimeZone(zoneName);
    }
}
