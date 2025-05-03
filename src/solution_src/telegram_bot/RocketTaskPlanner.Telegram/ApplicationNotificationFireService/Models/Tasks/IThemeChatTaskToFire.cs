namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks;

/// <summary>
/// Сообщение темы чата, которое нужно отправить
/// </summary>
public interface IThemeChatTaskToFire : ITaskToFire
{
    /// <summary>
    /// ID сообщения
    /// </summary>
    /// <returns>ID сообщения</returns>
    long SubjectId();

    /// <summary>
    /// ID чата
    /// </summary>
    /// <returns>ID чата</returns>
    long ChatId();

    /// <summary>
    /// Сообщение
    /// </summary>
    /// <returns>сообщение</returns>
    string Message();

    /// <summary>
    /// ID темы чата
    /// </summary>
    /// <returns>ID темы чата</returns>
    public int ThemeChatId();

    /// <summary>
    /// Дата создания сообщения
    /// </summary>
    /// <returns>Дата создания сообщения</returns>
    DateTime Created();

    /// <summary>
    /// Дата уведомления сообщения
    /// </summary>
    /// <returns>Дата уведомления сообщения</returns>
    DateTime Notified();

    /// <summary>
    /// Периодичность сообщения
    /// </summary>
    /// <returns>Периодичность сообщения</returns>
    bool IsPeriodic();
}
