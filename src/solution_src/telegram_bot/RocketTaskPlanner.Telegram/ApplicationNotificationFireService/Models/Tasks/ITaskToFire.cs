namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks;

public interface ITaskToFire
{
    Task<ITaskToFire> Fire();
}
