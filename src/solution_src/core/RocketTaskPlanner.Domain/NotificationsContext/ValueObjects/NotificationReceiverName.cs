namespace RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

public sealed record NotificationReceiverName
{
    public string Name { get; init; }

    private NotificationReceiverName() => Name = string.Empty;

    private NotificationReceiverName(string name) => Name = name;

    public static Result<NotificationReceiverName> Create(string? name) =>
        string.IsNullOrWhiteSpace(name)
            ? Result.Failure<NotificationReceiverName>("Название чата было пустым.")
            : new NotificationReceiverName(name);
}
