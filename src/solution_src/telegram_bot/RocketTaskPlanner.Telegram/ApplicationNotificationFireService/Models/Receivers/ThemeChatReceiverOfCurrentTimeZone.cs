using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Receivers;

/// <summary>
/// Получатели сообщений (темы чатов)
/// </summary>
public sealed class ThemeChatReceiverOfCurrentTimeZone
{
    /// <summary>
    ///     <inheritdoc cref="ThemeChatSubjectEntity"/>
    /// </summary>
    private readonly List<ThemeChatSubjectEntity> _subjects;

    public ThemeChatReceiverOfCurrentTimeZone(ReceiverThemeEntity theme) =>
        _subjects = theme.Subjects;

    /// <summary>
    ///     <inheritdoc cref="ThemeChatSubjectEntity"/>
    /// </summary>
    /// <returns>
    ///     <inheritdoc cref="ThemeChatSubjectEntity"/>
    /// </returns>
    public ThemeChatSubjectEntity[] Subjects() => _subjects.ToArray();
}
