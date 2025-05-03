namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks;

/// <summary>
/// Сообщение которое нужно отправить
/// </summary>
public interface IGeneralChatTaskToFire : ITaskToFire
{
    /// <summary>
    /// ID сообщения
    /// </summary>
    /// <returns>ID сообщения</returns>
    long SubjectId();

    /// <summary>
    /// ID чата
    /// </summary>
    /// <returns>id чата</returns>
    long ChatId();

    /// <summary>
    /// Сообщение
    /// </summary>
    /// <returns>сообщение</returns>
    string Message();

    /// <summary>
    /// Дата создания
    /// </summary>
    /// <returns>Дата создания</returns>
    DateTime Created();

    /// <summary>
    /// Дата уведомления
    /// </summary>
    /// <returns>Дата уведомления</returns>
    DateTime Notified();

    /// <summary>
    /// Периодическая или нет
    /// </summary>
    /// <returns>Периодичность</returns>
    bool IsPeriodic();
}
