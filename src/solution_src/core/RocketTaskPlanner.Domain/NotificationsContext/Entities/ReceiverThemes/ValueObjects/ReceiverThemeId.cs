namespace RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes.ValueObjects;

/// <summary>
/// ID темы основного чата
/// </summary>
public readonly record struct ReceiverThemeId
{
    public long Id { get; }

    public ReceiverThemeId() => Id = -1;

    private ReceiverThemeId(long id) => Id = id;

    public static Result<ReceiverThemeId> Create(long? id)
    {
        if (id == null)
            return Result.Failure<ReceiverThemeId>("Id темы некорректный.");
        return new ReceiverThemeId(id.Value);
    }
}
