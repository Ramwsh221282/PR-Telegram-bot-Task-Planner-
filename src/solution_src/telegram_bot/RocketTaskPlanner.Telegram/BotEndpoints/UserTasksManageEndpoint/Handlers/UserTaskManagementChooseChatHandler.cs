using PRTelegramBot.Models;
using PRTelegramBot.Utils;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Cache;
using RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Constants;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Handlers;

public sealed class UserTaskManagementChooseChatHandler : ITelegramBotHandler
{
    private readonly TelegramBotExecutionContext _context;

    public UserTaskManagementChooseChatHandler(TelegramBotExecutionContext context) =>
        _context = context;

    public string Command => HandlerNames.ChooseChatHandler;

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        var cache = _context.GetCacheInfo<UserTaskManagementCache>();
        if (cache.IsFailure)
            return;

        var chats = cache.Value.UserChats;

        List<KeyboardButton> menuList = [];
        foreach (var chat in chats)
        {
            var buttonContent = $"{chat.ChatName}";
            var button = new KeyboardButton(buttonContent);
            menuList.Add(button);
        }

        var menu = MenuGenerator.ReplyKeyboard(1, menuList);
        var options = new OptionMessage { MenuReplyKeyboardMarkup = menu };
        const string replyMessage = TaskManageConstants.SelectChatReply;
        await PRTelegramBot.Helpers.Message.Send(client, update, replyMessage, options);

        var nextHandler = _context.GetRequiredHandler(HandlerNames.OnChatChoseHandler);
        _context.AssignNextStep(update, nextHandler, cache.Value);
    }
}
