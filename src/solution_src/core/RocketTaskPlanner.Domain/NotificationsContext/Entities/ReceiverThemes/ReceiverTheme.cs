using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

namespace RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;

/// <summary>
/// Тема основного чата
/// </summary>
public sealed class ReceiverTheme
{
    /// <summary>
    /// Уведомления темы чата
    /// </summary>
    private readonly List<ThemeChatSubject> _subjects = [];
    public ReceiverThemeId Id { get; }

    /// <summary>
    /// Чат-обладатель темы (основной чат)
    /// </summary>
    public NotificationReceiver NotificationReceiver { get; } = null!;
    public NotificationReceiverId NotificationReceiverId { get; }
    public IReadOnlyList<ThemeChatSubject> Subjects => _subjects;

    private ReceiverTheme() { } // ef core

    internal ReceiverTheme(ReceiverThemeId id, NotificationReceiver receiver)
    {
        Id = id;
        NotificationReceiver = receiver;
        NotificationReceiverId = receiver.Id;
    }

    /// <summary>
    /// Добавление уведомления в тему чата
    /// </summary>
    /// <param name="id">Id уведомления</param>
    /// <param name="time">Информация о времени</param>
    /// <param name="period">Информация о периодичностии</param>
    /// <param name="message">Текст сообщения</param>
    /// <returns>Уведомление темы чата</returns>
    public ThemeChatSubject AddSubject(
        ReceiverSubjectId id,
        ReceiverSubjectTimeInfo time,
        ReceiverSubjectPeriodInfo period,
        ReceiverSubjectMessage message
    )
    {
        ThemeChatSubject subject = new(id, time, message, period, this);
        _subjects.Add(subject);
        return subject;
    }
}
