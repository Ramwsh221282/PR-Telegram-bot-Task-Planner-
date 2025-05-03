using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Receivers;

/// <summary>
/// Получатели сообщений (темы чатов)
/// </summary>
public sealed class ThemeChatReceiverOfCurrentTimeZone
{
    private readonly List<ThemeChatSubjectEntity> _subjects;

    public ThemeChatReceiverOfCurrentTimeZone(ReceiverThemeEntity theme) =>
        _subjects = theme.Subjects;

    /// <summary>
    /// Сообщения
    /// </summary>
    /// <returns></returns>
    public ThemeChatSubjectEntity[] Subjects() => _subjects.ToArray();
}
