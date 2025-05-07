namespace RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Constants;

/// <summary>
/// Дополнительные константы для контекста /my_tasks
/// Используются для Reply сообщений и текста кнопок.
/// </summary>
public static class TaskManageConstants
{
    /// <summary>
    /// Reply сообщение для выбора чата
    /// </summary>
    public const string SelectChatReply = "Выберите чат";

    /// <summary>
    /// Reply сообщение для выбора типа задач
    /// </summary>
    public const string SelectTaskTypeReply = "Выберите тип задач";

    /// <summary>
    /// Текст для кнопки "Периодические" (задачи)
    /// </summary>
    public const string PeriodicTaskReply = "Периодические";

    /// <summary>
    /// Текст для кнопки "Не периодические" (задачи)
    /// </summary>
    public const string NonPeriodicTaskReply = "Не периодические";

    /// <summary>
    /// Текст для кнопки "Вернуться в список задач".
    /// </summary>
    public const string BackToList = "Вернуться в список";

    /// <summary>
    /// Текст для кнопки "Удалить задачу"
    /// </summary>
    public const string DeleteTask = "Удалить задачу";

    /// <summary>
    /// Текст для кнопки "Назад в типы задач"
    /// </summary>
    public const string BackToTaskTypes = "Назад в типы задач";

    /// <summary>
    /// Текст для кнопки "Назад в список чатов"
    /// </summary>
    public const string BackToChats = "Назад в список чатов";
}
