using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes.ValueObjects;

namespace RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;

public sealed class ThemeChatSubject : ReceiverSubject
{
    public ReceiverTheme Theme { get; } = null!;
    public ReceiverThemeId ThemeId { get; } = default!;

    private ThemeChatSubject() { } // ef core

    internal ThemeChatSubject(
        ReceiverSubjectId id,
        ReceiverSubjectTimeInfo time,
        ReceiverSubjectMessage message,
        ReceiverSubjectPeriodInfo period,
        ReceiverTheme theme
    )
    {
        Id = id;
        TimeInfo = time;
        Message = message;
        Period = period;
        Theme = theme;
        ThemeId = theme.Id;
    }
}
