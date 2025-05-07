namespace RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;

/// <summary>
/// ID уведомления основного чата
/// </summary>
public readonly record struct ReceiverSubjectId
{
    public long Id { get; }

    public ReceiverSubjectId() => Id = -1;

    private ReceiverSubjectId(long id) => Id = id;

    public static Result<ReceiverSubjectId> Create(long? id)
    {
        if (id == null)
            return Result.Failure<ReceiverSubjectId>("Id сообщения некорректный.");

        return new ReceiverSubjectId(id.Value);
    }
}
