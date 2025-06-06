using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

namespace RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;

/// <summary>
/// Уведомление основного чата
/// </summary>
public sealed class GeneralChatReceiverSubject : ReceiverSubject
{
    /// <summary>
    /// Чат-обладатель сообщения
    /// </summary>
    public NotificationReceiver Receiver { get; } = null!;

    /// <summary>
    /// ID основного чата уведомлений
    /// </summary>
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
