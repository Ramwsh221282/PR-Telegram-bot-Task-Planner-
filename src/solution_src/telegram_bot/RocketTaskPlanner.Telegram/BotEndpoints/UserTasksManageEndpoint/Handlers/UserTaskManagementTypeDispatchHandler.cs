using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Cache;
using RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Constants;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Handlers;

public sealed class UserTaskManagementTypeDispatchHandler : ITelegramBotHandler
{
    private readonly TelegramBotExecutionContext _context;

    public UserTaskManagementTypeDispatchHandler(TelegramBotExecutionContext context) =>
        _context = context;

    public string Command => HandlerNames.TaskTypeDispatcher;

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        var cache = _context.GetCacheInfo<UserTaskManagementCache>();
        if (cache.IsFailure)
            return;

        var message = update.GetMessage();
        if (message.IsFailure)
            return;

        var cacheValue = cache.Value;
        if (message.Value == TaskManageConstants.BackToChats)
        {
            await BackToChats(client, update, cacheValue);
            return;
        }

        bool isPeriodic = message.Value == TaskManageConstants.PeriodicTaskReply;
        cacheValue = cacheValue with { IsPeriodicView = isPeriodic };

        await client.ClearMenu(update, $"Выбранный тип задач: {message.Value}");

        const string nextHandlerName = HandlerNames.TasksViewHandler;
        var nextHandler = _context.GetRequiredHandler(nextHandlerName);
        await _context.AssignAndRun(client, update, nextHandler, cacheValue);
    }

    private async Task BackToChats(
        ITelegramBotClient client,
        Update update,
        UserTaskManagementCache cache
    )
    {
        var handlerName = HandlerNames.ChooseChatHandler;
        var handler = _context.GetRequiredHandler(handlerName);
        await _context.AssignAndRun(client, update, handler, cache.ClearSelectedChat());
    }
}
