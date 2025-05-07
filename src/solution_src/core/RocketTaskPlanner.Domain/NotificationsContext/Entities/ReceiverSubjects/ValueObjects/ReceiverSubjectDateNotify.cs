namespace RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;

/// <summary>
/// Дата уведомления основного чата
/// </summary>
public readonly record struct ReceiverSubjectDateNotify
{
    public DateTime Value { get; }

    public ReceiverSubjectDateNotify() => Value = DateTime.MinValue;

    public ReceiverSubjectDateNotify(DateTime dateTime) => Value = dateTime;
}
