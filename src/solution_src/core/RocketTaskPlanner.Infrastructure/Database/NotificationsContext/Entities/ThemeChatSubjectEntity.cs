using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;

namespace RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Entities;

/// <summary>
/// Dao модель уведомления темы чата
/// </summary>
public sealed class ThemeChatSubjectEntity
{
    public long ThemeChatSubjectId { get; set; }
    public long ThemeId { get; set; }
    public int SubjectPeriodic { get; set; }
    public DateTime SubjectCreated { get; set; }
    public DateTime SubjectNotify { get; set; }
    public string SubjectMessage { get; set; } = string.Empty;

    public ThemeChatSubject ToSubject(ReceiverTheme theme)
    {
        ReceiverSubjectId id = ReceiverSubjectId.Create(ThemeChatSubjectId).Value;
        ReceiverSubjectDateCreated created = new(SubjectCreated);
        ReceiverSubjectDateNotify notify = new(SubjectNotify);
        ReceiverSubjectPeriodInfo period = CreatePeriodInfo(created, notify);
        ReceiverSubjectTimeInfo time = new(created, notify);
        ReceiverSubjectMessage message = ReceiverSubjectMessage.Create(SubjectMessage).Value;
        return theme.AddSubject(id, time, period, message);
    }

    private ReceiverSubjectPeriodInfo CreatePeriodInfo(
        ReceiverSubjectDateCreated created,
        ReceiverSubjectDateNotify notify
    ) =>
        SubjectPeriodic == 0
            ? new ReceiverSubjectPeriodInfo(false)
            : new ReceiverSubjectPeriodInfo(true);
}
