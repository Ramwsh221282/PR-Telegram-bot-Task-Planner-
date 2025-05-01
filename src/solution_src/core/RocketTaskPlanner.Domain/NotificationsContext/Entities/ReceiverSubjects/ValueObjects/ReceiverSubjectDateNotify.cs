namespace RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;

public readonly record struct ReceiverSubjectDateNotify
{
    public DateTime Value { get; }

    public ReceiverSubjectDateNotify() => Value = DateTime.MinValue;

    public ReceiverSubjectDateNotify(DateTime dateTime) => Value = dateTime;
}
