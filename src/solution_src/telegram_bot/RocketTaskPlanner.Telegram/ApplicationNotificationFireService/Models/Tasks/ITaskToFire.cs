namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks;

/// <summary>
/// Сообщение которое нужно отправить
/// </summary>
public interface ITaskToFire
{
    /// <summary>
    /// Отправка сообщения
    /// </summary>
    /// <returns></returns>
    Task<ITaskToFire> Fire();
}
