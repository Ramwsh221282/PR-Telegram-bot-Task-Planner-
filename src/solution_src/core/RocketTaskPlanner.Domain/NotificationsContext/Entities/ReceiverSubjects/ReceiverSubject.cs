using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;

namespace RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;

public abstract class ReceiverSubject
{
    public ReceiverSubjectId Id { get; protected set; } = default!;
    public ReceiverSubjectTimeInfo TimeInfo { get; protected set; } = default!;
    public ReceiverSubjectMessage Message { get; protected set; } = default!;
    public ReceiverSubjectPeriodInfo Period { get; protected set; } = default!;
}
