using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Cache;
using RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Constants;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Handlers;

public sealed class UserTaskManagementOnChatChosenHandler : ITelegramBotHandler
{
    private readonly TelegramBotExecutionContext _context;
    public string Command => HandlerNames.OnChatChoseHandler;

    public UserTaskManagementOnChatChosenHandler(TelegramBotExecutionContext context) =>
        _context = context;

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

        var nextHandler = _context.GetRequiredHandler(HandlerNames.ChooseTaskTypesHandler);
        await _context.AssignAndRun(client, update, nextHandler, cacheWithChat);
    }
}
