using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities;

/// <summary>
/// Dao модель уведомления основного чата
/// </summary>
public sealed class GeneralChatSubjectEntity
{
    public long GeneralChatSubjectId { get; set; }
    public long GeneralChatId { get; set; }
    public int SubjectPeriodic { get; set; }
    public DateTime SubjectCreated { get; set; }
    public DateTime SubjectNotify { get; set; }
    public string SubjectMessage { get; set; } = string.Empty;

    public GeneralChatReceiverSubject ToSubject(NotificationReceiver receiver)
    {
        ReceiverSubjectId id = ReceiverSubjectId.Create(GeneralChatSubjectId).Value;
        ReceiverSubjectDateCreated created = new(SubjectCreated);
        ReceiverSubjectDateNotify notify = new ReceiverSubjectDateNotify(SubjectNotify);
        ReceiverSubjectPeriodInfo period = CreatePeriodInfo(created, notify);
        ReceiverSubjectTimeInfo time = new(created, notify);
        ReceiverSubjectMessage message = ReceiverSubjectMessage.Create(SubjectMessage).Value;
        return receiver.AddSubject(id, time, period, message);
    }

    private ReceiverSubjectPeriodInfo CreatePeriodInfo(
        ReceiverSubjectDateCreated created,
        ReceiverSubjectDateNotify notify
    ) =>
        SubjectPeriodic == 0
            ? new ReceiverSubjectPeriodInfo(false)
            : new ReceiverSubjectPeriodInfo(true);
}
