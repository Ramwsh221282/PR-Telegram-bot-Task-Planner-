using PRTelegramBot.Models;
using PRTelegramBot.Utils;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Cache;
using RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Constants;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Handlers;

public sealed class UserTaskManagementChooseTaskTypesHandler : ITelegramBotHandler
{
    private readonly TelegramBotExecutionContext _context;

    public UserTaskManagementChooseTaskTypesHandler(TelegramBotExecutionContext context) =>
        _context = context;

    public string Command => HandlerNames.ChooseTaskTypesHandler;

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        var cache = _context.GetCacheInfo<UserTaskManagementCache>();
        if (cache.IsFailure)
            return;

        List<KeyboardButton> menuList =
        [
            new(TaskManageConstants.PeriodicTaskReply),
            new(TaskManageConstants.NonPeriodicTaskReply),
            new(TaskManageConstants.BackToChats),
        ];
        var menu = MenuGenerator.ReplyKeyboard(1, menuList);
        var options = new OptionMessage { MenuReplyKeyboardMarkup = menu };
        const string replyMessage = TaskManageConstants.SelectTaskTypeReply;

        await PRTelegramBot.Helpers.Message.Send(client, update, replyMessage, options);

        var nextHandler = _context.GetRequiredHandler(HandlerNames.TaskTypeDispatcher);
        _context.AssignNextStep(update, nextHandler, cache.Value);
    }
}
