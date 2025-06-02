using RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Entities;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks.GeneralChatTasks;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks.ThemeChatTasks;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Receivers;

/// <summary>
/// Получатели сообщений временных зон (основные чаты)
/// </summary>
public sealed class GeneralChatReceiverOfCurrentTimeZone
{
    /// <summary>
    ///     <inheritdoc cref="NotificationReceiverEntity"/>
    /// </summary>
    private readonly NotificationReceiverEntity _receiver;

    public GeneralChatReceiverOfCurrentTimeZone(NotificationReceiverEntity receiver) =>
        _receiver = receiver;

    /// <summary>
    ///     <inheritdoc cref="IGeneralChatTaskToFire"/>
    /// </summary>
    /// <returns>
    ///     <inheritdoc cref="IGeneralChatTaskToFire"/>
    /// </returns>
    public IGeneralChatTaskToFire[] GeneralChatTasksToFire() =>
        [.. _receiver.ReceiverSubjects.Select(s => new GeneralChatTaskToFire(s))];

    /// <summary>
    ///     <inheritdoc cref="IThemeChatTaskToFire"/>
    /// </summary>
    /// <returns>
    ///     <inheritdoc cref="IThemeChatTaskToFire"/>
    /// </returns>
    public IThemeChatTaskToFire[] ThemeChatTasksToFire() =>
        [
            .. _receiver
                .ReceiverThemes.SelectMany(th => th.Subjects)
                .Select(s => new ThemeChatTaskToFire(s, _receiver.ReceiverId)),
        ];
}
