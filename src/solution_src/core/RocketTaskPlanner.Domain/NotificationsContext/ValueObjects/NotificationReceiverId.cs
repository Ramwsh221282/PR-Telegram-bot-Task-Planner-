namespace RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

public readonly record struct NotificationReceiverId
{
    public long Id { get; }

    public NotificationReceiverId() => Id = -1;

    private NotificationReceiverId(long id) => Id = id;

    public static Result<NotificationReceiverId> Create(long? id)
    {
        if (id == null)
            return Result.Failure<NotificationReceiverId>("Id чата некорректный.");

        return new NotificationReceiverId(id.Value);
        ;
    }

    public static Result<NotificationReceiverId> Create(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return Result.Failure<NotificationReceiverId>("Id чата некорректный");

        if (!long.TryParse(id, out long parsedId))
            return Result.Failure<NotificationReceiverId>("Id чата некорректный");

        return Create(parsedId);
    }
}
