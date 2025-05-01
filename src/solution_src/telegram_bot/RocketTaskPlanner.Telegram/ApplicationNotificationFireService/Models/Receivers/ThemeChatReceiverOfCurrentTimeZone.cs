using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Receivers;

public sealed class ThemeChatReceiverOfCurrentTimeZone
{
    private readonly List<ThemeChatSubjectEntity> _subjects;

    public ThemeChatReceiverOfCurrentTimeZone(ReceiverThemeEntity theme) =>
        _subjects = theme.Subjects;

    public ThemeChatSubjectEntity[] Subjects() => _subjects.ToArray();
}
