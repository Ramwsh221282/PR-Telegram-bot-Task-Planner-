using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

namespace RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;

public sealed class GeneralChatReceiverSubject : ReceiverSubject
{
    public NotificationReceiver Receiver { get; } = null!;
    public NotificationReceiverId GeneralChatId { get; } = default!;

    private GeneralChatReceiverSubject() { } // ef core

    internal GeneralChatReceiverSubject(
        ReceiverSubjectId id,
        ReceiverSubjectTimeInfo time,
        ReceiverSubjectMessage message,
        ReceiverSubjectPeriodInfo period,
        NotificationReceiver receiver
    )
    {
        Id = id;
        TimeInfo = time;
        Message = message;
        Period = period;
        Receiver = receiver;
        GeneralChatId = receiver.Id;
    }
}
