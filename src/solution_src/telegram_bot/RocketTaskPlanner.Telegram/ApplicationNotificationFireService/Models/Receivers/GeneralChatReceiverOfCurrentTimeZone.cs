using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks.GeneralChatTasks;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks.ThemeChatTasks;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Receivers;

public sealed class GeneralChatReceiverOfCurrentTimeZone
{
    private readonly NotificationReceiverEntity _receiver;

    public GeneralChatReceiverOfCurrentTimeZone(NotificationReceiverEntity receiver) =>
        _receiver = receiver;

    public IGeneralChatTaskToFire[] TasksToFire() =>
        [.. _receiver.ReceiverSubjects.Select(s => new GeneralChatTaskToFire(s))];

    public IThemeChatTaskToFire[] ThemeChatUnfiredTasks() =>
        [
            .. _receiver
                .ReceiverThemes.SelectMany(th => th.Subjects)
                .Select(s => new ThemeChatTaskToFire(s, _receiver.ReceiverId)),
        ];
}
