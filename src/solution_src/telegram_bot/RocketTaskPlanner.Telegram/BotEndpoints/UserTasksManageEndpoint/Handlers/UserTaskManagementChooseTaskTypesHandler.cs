using PRTelegramBot.Models;
using PRTelegramBot.Utils;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Cache;
using RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Constants;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Handlers;

/// <summary>
/// Обработчик создания меню типов задач.
/// </summary>
public sealed class UserTaskManagementChooseTaskTypesHandler : ITelegramBotHandler
{
    /// <summary>
    /// <inheritdoc cref="TelegramBotExecutionContext"/>
    /// </summary>
    private readonly TelegramBotExecutionContext _context;

    public UserTaskManagementChooseTaskTypesHandler(TelegramBotExecutionContext context) =>
        _context = context;

    /// <summary>
    /// <inheritdoc cref="ITelegramBotHandler.Command"/>
    /// </summary>
    public string Command => HandlerNames.ChooseTaskTypesHandler;

    /// <summary>
    /// <inheritdoc cref="ITelegramBotHandler.Handle"/>
    /// </summary>
    public async Task Handle(ITelegramBotClient client, Update update)
    {
        // получение кеша из контекста
        var cache = _context.GetCacheInfo<UserTaskManagementCache>();
        if (cache.IsFailure)
            return;

        // создание меню с кнопками: "Периодические", "Не периодические", "Назад в выбор чатов"
        List<KeyboardButton> menuList =
        [
            new(TaskManageConstants.PeriodicTaskReply),
            new(TaskManageConstants.NonPeriodicTaskReply),
            new(TaskManageConstants.BackToChats),
        ];
        var menu = MenuGenerator.ReplyKeyboard(1, menuList);
        var options = new OptionMessage { MenuReplyKeyboardMarkup = menu };
        const string replyMessage = TaskManageConstants.SelectTaskTypeReply;

        // отправка меню
        await PRTelegramBot.Helpers.Message.Send(client, update, replyMessage, options);

        // получения обработчика нажатия на выбор типа задач.
        var nextHandler = _context.GetRequiredHandler(HandlerNames.TaskTypeDispatcher);

        // назначение и немедленный запуск обработчика
        _context.AssignNextStep(update, nextHandler, cache.Value);
    }
}
