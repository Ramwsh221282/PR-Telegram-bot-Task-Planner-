using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

namespace RocketTaskPlanner.Domain.NotificationsContext;

public sealed class NotificationReceiver
{
    private readonly List<GeneralChatReceiverSubject> _subjects = [];
    private readonly List<ReceiverTheme> _themes = [];
    public required NotificationReceiverId Id { get; init; }
    public required NotificationReceiverName Name { get; init; }
    public required NotificationReceiverTimeZone TimeZone { get; init; }
    public IReadOnlyList<GeneralChatReceiverSubject> Subjects => _subjects;
    public IReadOnlyList<ReceiverTheme> Themes => _themes;

    public Result<ReceiverTheme> AddTheme(ReceiverThemeId id)
    {
        if (_themes.Any(t => t.Id == id))
            return Result.Failure<ReceiverTheme>(
                $"Тема с ID: {Id.Id} уже присутствует в чате: ID: {Id.Id} Name: {Name.Name}"
            );

        ReceiverTheme theme = new(id, this);
        _themes.Add(theme);
        return theme;
    }

    public GeneralChatReceiverSubject AddSubject(
        ReceiverSubjectId id,
        ReceiverSubjectTimeInfo time,
        ReceiverSubjectPeriodInfo period,
        ReceiverSubjectMessage message
    )
    {
        GeneralChatReceiverSubject subject = new(id, time, message, period, this);
        _subjects.Add(subject);
        return subject;
    }

    public Result<ReceiverTheme> RemoveTheme(ReceiverThemeId themeId)
    {
        ReceiverTheme? theme = _themes.FirstOrDefault(t => t.Id == themeId);

        if (theme == null)
            return Result.Failure<ReceiverTheme>(
                $"Тема с ID: {themeId.Id} не найдена в чате: ID: {Id} {Name.Name}"
            );

        _themes.Remove(theme);
        return theme;
    }
}
