using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

namespace RocketTaskPlanner.Domain.NotificationsContext;

/// <summary>
/// Получатель уведомлений (основной чат)
/// </summary>
public sealed class NotificationReceiver
{
    /// <summary>
    /// Список уведомлений основного чата
    /// </summary>
    private readonly List<GeneralChatReceiverSubject> _subjects = [];

    /// <summary>
    /// Список тем основного чата
    /// </summary>
    private readonly List<ReceiverTheme> _themes = [];
    public required NotificationReceiverId Id { get; init; }

    /// <summary>
    /// Название основного чата
    /// </summary>
    public required NotificationReceiverName Name { get; init; }

    /// <summary>
    /// Временная зона основного чата
    /// </summary>
    public required NotificationReceiverTimeZone TimeZone { get; init; }
    public IReadOnlyList<GeneralChatReceiverSubject> Subjects => _subjects;
    public IReadOnlyList<ReceiverTheme> Themes => _themes;

    /// <summary>
    /// Добавление темы в основной чат. Success если темы ещё нет в чате. Failure если тема есть в чате.
    /// </summary>
    /// <param name="id">Id темы</param>
    /// <returns>Success если тема добавлена. Failure если не добавлена.</returns>
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

    /// <summary>
    /// Добавление уведомления в основной чат.
    /// </summary>
    /// <param name="id">Id сообщения</param>
    /// <param name="time">Информация о времени(создания и уведомления)</param>
    /// <param name="period">Информация о периодичности</param>
    /// <param name="message">Текст сообщения</param>
    /// <returns></returns>
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

    /// <summary>
    /// Удаление темы из основного чата
    /// </summary>
    /// <param name="themeId">Id темы</param>
    /// <returns>Success если тема была найдена и удалена. Failure если не была найдена.</returns>
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
