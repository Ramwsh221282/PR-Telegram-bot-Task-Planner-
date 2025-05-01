namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks;

public interface IThemeChatTaskToFire : ITaskToFire
{
    long SubjectId();
    long ChatId();
    string Message();
    public int ThemeChatId();
    DateTime Created();
    DateTime Notified();
    bool IsPeriodic();
}
