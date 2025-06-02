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
/// Обработчик создания меню выбора чата
/// </summary>
public sealed class UserTaskManagementChooseChatHandler : ITelegramBotHandler
{
    /// <summary>
    /// <inheritdoc cref="TelegramBotExecutionContext"/>
    /// </summary>
    private readonly TelegramBotExecutionContext _context;

    public UserTaskManagementChooseChatHandler(TelegramBotExecutionContext context) =>
        _context = context;

    /// <summary>
    /// <inheritdoc cref="ITelegramBotHandler.Command"/>
    /// </summary>
    public string Command => HandlerNames.ChooseChatHandler;

    /// <summary>
    /// <inheritdoc cref="ITelegramBotHandler.Handle"/>
    /// </summary>
    public async Task Handle(ITelegramBotClient client, Update update)
    {
        // получение кеша из контекста.
        var cache = _context.GetCacheInfo<UserTaskManagementCache>();
        if (cache.IsFailure)
            return;

        // получения списка пользовательских чатов из кеша.
        var chats = cache.Value.UserChats;

        // создание меню выбора чатов
        List<KeyboardButton> menuList = [];
        foreach (var chat in chats)
        {
            var buttonContent = $"{chat.MetadataInformation()}";
            var button = new KeyboardButton(buttonContent);
            menuList.Add(button);
        }

        var menu = MenuGenerator.ReplyKeyboard(1, menuList);
        var options = new OptionMessage { MenuReplyKeyboardMarkup = menu };

        // отправка меню
        const string replyMessage = TaskManageConstants.SelectChatReply;
        await PRTelegramBot.Helpers.Message.Send(client, update, replyMessage, options);

        // получение обработчика выбора чата из меню.
        const string nextHandlerName = HandlerNames.OnChatChoseHandler;
        var nextHandler = _context.GetRequiredHandler(nextHandlerName);

        // назначение следующего обработчика в контесте.
        _context.AssignNextStep(update, nextHandler, cache.Value);
    }
}
