using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Cache;
using RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Constants;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Handlers;

/// <summary>
/// Обработчик нажатия на выбранный чат
/// </summary>
public sealed class UserTaskManagementOnChatChosenHandler : ITelegramBotHandler
{
    /// <summary>
    /// <inheritdoc cref="TelegramBotExecutionContext"/>
    /// </summary>
    private readonly TelegramBotExecutionContext _context;

    /// <summary>
    /// <inheritdoc cref="ITelegramBotHandler.Command"/>
    /// </summary>
    public string Command => HandlerNames.OnChatChoseHandler;

    public UserTaskManagementOnChatChosenHandler(TelegramBotExecutionContext context) =>
        _context = context;

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

        await client.ClearMenu(update, $"Выбран чат: {message.Value}");

        var requiredChat = cache.Value.GetChat(message.Value);
        var cacheWithChat = cache.Value.SetSelectedChat(requiredChat);

        // назначение обработчика меню выбора типа задач следующим.
        var nextHandler = _context.GetRequiredHandler(HandlerNames.ChooseTaskTypesHandler);
        await _context.AssignAndRun(client, update, nextHandler, cacheWithChat);
    }
}
