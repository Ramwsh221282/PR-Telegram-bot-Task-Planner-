using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.NotificationsContext.Features.RemoveChatSubject;
using RocketTaskPlanner.Application.NotificationsContext.Features.RemoveThemeSubject;
using RocketTaskPlanner.Application.NotificationsContext.Visitor;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetPagedChatSubjects;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Cache;
using RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Constants;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Handlers;

/// <summary>
/// Обработчик для меню конкретного объявления.
/// Обрабатывает кнопки "Вернуться назад" и "Удалить задачу".
/// </summary>
public sealed class UserTaskManagementConcreteTaskViewHandler : ITelegramBotHandler
{
    /// <summary>
    /// <inheritdoc cref="TelegramBotExecutionContext"/>
    /// </summary>
    private readonly TelegramBotExecutionContext _context;

    /// <summary>
    /// <inheritdoc cref="INotificationUseCaseVisitor"/>
    /// </summary>
    private readonly INotificationUseCaseVisitor _useCases;

    public UserTaskManagementConcreteTaskViewHandler(
        TelegramBotExecutionContext context,
        INotificationUseCaseVisitor useCases
    )
    {
        _context = context;
        _useCases = useCases;
    }

    /// <summary>
    /// <inheritdoc cref="ITelegramBotHandler.Command"/>
    /// </summary>
    public string Command => HandlerNames.ConcreteTaskViewHandler;

    /// <summary>
    /// <inheritdoc cref="ITelegramBotHandler.Handle"/>
    /// </summary>
    public async Task Handle(ITelegramBotClient client, Update update)
    {
        var cache = _context.GetCacheInfo<UserTaskManagementCache>();
        if (cache.IsFailure)
            return;

        var message = update.GetMessage();
        if (message.IsFailure)
            return;

        if (message.Value == TaskManageConstants.DeleteTask)
            await DeleteTask(client, update, cache.Value);
        if (message.Value == TaskManageConstants.BackToList)
            await GoBackToList(client, update, cache.Value);
    }

    /// <summary>
    /// Вернуться в список задач
    /// </summary>
    /// <param name="client">Telegram bot клиент для взаимодействия с Telegram.</param>
    /// <param name="update">Последнее событие</param>
    /// <param name="cache">Кэш</param>
    private async Task GoBackToList(
        ITelegramBotClient client,
        Update update,
        UserTaskManagementCache cache
    )
    {
        const string handlerName = HandlerNames.TasksViewHandler;
        var nextHandler = _context.GetRequiredHandler(handlerName);

        await PRTelegramBot.Helpers.Message.Send(client, update, $"Назад в меню");
        await _context.AssignAndRun(client, update, nextHandler, cache);
    }

    /// <summary>
    /// Удалить задачу
    /// </summary>
    /// <param name="client">Telegram bot клиент для взаимодействия с Telegram.</param>
    /// <param name="update">Последнее событие</param>
    /// <param name="cache">Кэш</param>
    private async Task DeleteTask(
        ITelegramBotClient client,
        Update update,
        UserTaskManagementCache cache
    )
    {
        var subject = cache.GetSelectedSubject();
        var subjectId = subject.GetSubjectId();
        var subjectText = subject.GetSubjectMessage();
        bool isThemeSubject = subject.IsThemeSubject();

        await PRTelegramBot.Helpers.Message.Send(client, update, "Удаляю задачу...");

        // вызов бизнес логики.
        Result deletion = isThemeSubject
            ? await _useCases.Visit(new RemoveThemeSubjectUseCase(subjectId))
            : await _useCases.Visit(new RemoveChatSubjectUseCase(subjectId));

        // если бизнес логика выдала ошибка, вернуться в спиоск задач.
        if (deletion.IsFailure)
        {
            await PRTelegramBot.Helpers.Message.Send(client, update, deletion.Error);
            await GoBackToList(client, update, cache);
            return;
        }

        await PRTelegramBot.Helpers.Message.Send(
            client,
            update,
            $"Удалена задача: {subjectId} {subjectText}"
        );

        // после удаления задачи вернуться в список задач.
        await GoBackToList(client, update, cache.ClearSelectedSubject());
    }
}
