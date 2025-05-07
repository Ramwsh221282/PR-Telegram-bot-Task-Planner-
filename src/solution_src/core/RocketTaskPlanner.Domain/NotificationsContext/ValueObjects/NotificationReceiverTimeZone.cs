namespace RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

/// <summary>
/// Временная зона основного чата уведомлений
/// </summary>
public sealed record class NotificationReceiverTimeZone
{
    /// <summary>
    /// Временная зона название
    /// </summary>
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
