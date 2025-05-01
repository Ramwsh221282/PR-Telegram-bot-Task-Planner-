namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks;

public interface IGeneralChatTaskToFire : ITaskToFire
{
    long SubjectId();
    long ChatId();
    string Message();
    DateTime Created();
    DateTime Notified();
    bool IsPeriodic();
}
