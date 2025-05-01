namespace RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;

public readonly record struct ReceiverSubjectPeriodInfo
{
    public bool IsPeriodic { get; }

    public ReceiverSubjectPeriodInfo()
    {
        IsPeriodic = false;
    }

    public ReceiverSubjectPeriodInfo(bool isPeriodic)
    {
        IsPeriodic = isPeriodic;
    }
}
