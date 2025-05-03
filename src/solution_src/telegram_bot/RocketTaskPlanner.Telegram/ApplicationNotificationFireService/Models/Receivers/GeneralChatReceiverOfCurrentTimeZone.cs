using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks.GeneralChatTasks;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks.ThemeChatTasks;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Receivers;

/// <summary>
/// Получатели сообщений временных зон (основные чаты)
/// </summary>
public sealed class GeneralChatReceiverOfCurrentTimeZone
{
    private readonly NotificationReceiverEntity _receiver;

    public GeneralChatReceiverOfCurrentTimeZone(NotificationReceiverEntity receiver) =>
        _receiver = receiver;

    /// <summary>
    /// Сообщения для основного чата
    /// </summary>
    /// <returns>Сообщения основного чата, которые нужно отправить</returns>
    public IGeneralChatTaskToFire[] TasksToFire() =>
        [.. _receiver.ReceiverSubjects.Select(s => new GeneralChatTaskToFire(s))];

    /// <summary>
    /// Сообщения для тем чатов
    /// </summary>
    /// <returns>Сообщения для тем чатов, которые нужно отправить</returns>
    public IThemeChatTaskToFire[] ThemeChatUnfiredTasks() =>
        [
            .. _receiver
                .ReceiverThemes.SelectMany(th => th.Subjects)
                .Select(s => new ThemeChatTaskToFire(s, _receiver.ReceiverId)),
        ];
}
