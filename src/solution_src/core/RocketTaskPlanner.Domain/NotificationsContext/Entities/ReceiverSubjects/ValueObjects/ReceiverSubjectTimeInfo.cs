namespace RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;

/// <summary>
/// Информация о времени уведомления основного чата
/// </summary>
public readonly record struct ReceiverSubjectTimeInfo
{
    public ReceiverSubjectDateCreated Created { get; }
    public ReceiverSubjectDateNotify Notify { get; }

    public ReceiverSubjectTimeInfo()
    {
        Created = new();
        Notify = new();
    }

    public ReceiverSubjectTimeInfo(
        ReceiverSubjectDateCreated created,
        ReceiverSubjectDateNotify notify
    )
    {
        Created = created;
        Notify = notify;
    }
}
