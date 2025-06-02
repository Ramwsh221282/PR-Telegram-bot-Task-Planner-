using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Entities;

/// <summary>
/// Dao модель темы основного чата
/// </summary>
public sealed class ReceiverThemeEntity
{
    public long ThemeId { get; set; }
    public long ReceiverId { get; set; }
    public List<ThemeChatSubjectEntity> Subjects { get; set; } = [];

    public ReceiverThemeEntity() { }

    public ReceiverThemeEntity(ReceiverThemeEntity entity)
    {
        ThemeId = entity.ThemeId;
        ReceiverId = entity.ReceiverId;
        Subjects = entity.Subjects;
    }

    public void TryAddSubject(ThemeChatSubjectEntity subject)
    {
        if (
            subject.ThemeId == ThemeId
            && Subjects.All(s => s.ThemeChatSubjectId != subject.ThemeChatSubjectId)
        )
            Subjects.Add(subject);
    }

    public ReceiverTheme ToTheme(NotificationReceiver receiver)
    {
        ReceiverThemeId id = ReceiverThemeId.Create(ThemeId).Value;
        ReceiverTheme theme = receiver.AddTheme(id).Value;
        return theme;
    }
}
