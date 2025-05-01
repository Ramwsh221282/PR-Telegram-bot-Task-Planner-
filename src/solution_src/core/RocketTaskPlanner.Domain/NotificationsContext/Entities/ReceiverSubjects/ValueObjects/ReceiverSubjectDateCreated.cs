using RocketTaskPlanner.Utilities.DateExtensions;

namespace RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;

public readonly record struct ReceiverSubjectDateCreated
{
    public DateTime Value { get; }

    public ReceiverSubjectDateCreated() => Value = DateTime.MinValue;

    public ReceiverSubjectDateCreated(DateTime value)
    {
        Value = value;
    }

    public string DateString => Value.AsString();
}
