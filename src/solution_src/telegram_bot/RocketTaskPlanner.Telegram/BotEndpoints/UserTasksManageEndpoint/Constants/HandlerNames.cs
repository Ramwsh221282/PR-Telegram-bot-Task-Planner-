namespace RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Constants;

/// <summary>
/// Константы обработчиков контекста команды /my_tasks
/// </summary>
public static class HandlerNames
{
    /// <summary>
    /// Название обработчика валидации
    /// </summary>
    public const string ValidationHandler = "validation";

    /// <summary>
    /// Название обработчика выбора задач
    /// </summary>
    public const string ChooseTaskTypesHandler = "choose-task-types";

    /// <summary>
    /// Название обработчика выбранного типа задач
    /// </summary>
    public const string TaskTypeDispatcher = "choose-task-types-dispatch";

    /// <summary>
    /// Название обработчика выбора чата
    /// </summary>
    public const string ChooseChatHandler = "choose-chat";

    /// <summary>
    /// Название обработчика выбранного чата
    /// </summary>
    public const string OnChatChoseHandler = "on-chat-chose";

    /// <summary>
    /// Название обработчика меню списка задач
    /// </summary>
    public const string TasksViewHandler = "tasks-view";

    /// <summary>
    /// Название обработчика меню конкретной задачи
    /// </summary>
    public const string ConcreteTaskViewHandler = "concrete-task-view";
}
