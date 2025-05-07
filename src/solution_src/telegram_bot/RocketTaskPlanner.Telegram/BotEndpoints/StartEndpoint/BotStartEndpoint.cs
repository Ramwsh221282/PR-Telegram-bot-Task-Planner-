using CSharpFunctionalExtensions;
using PRTelegramBot.Attributes;
using PRTelegramBot.Extensions;
using PRTelegramBot.Models.Enums;
using RocketTaskPlanner.Telegram.BotExtensions;
using SQLitePCL;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.StartEndpoint;

/// <summary>
/// Endpoint обработки для команды /start
/// </summary>
[BotHandler]
public sealed class BotStartEndpoint
{
    private const string _messageOnStart = """
        Вас приветствует Telegram Bot Task Planner.
        Для получения информации как работать с ботом -
        Вызовите команду /bot_info
        """;

    /// <summary>
    /// Точка входа в команду /start
    /// </summary>
    /// <param name="client">Telegram bot клиент для общения с Telegram.</param>
    /// <param name="update">Последнее событие.</param>
    [SlashHandler(CommandComparison.Equals, StringComparison.OrdinalIgnoreCase, ["/start"])]
    public async Task OnStart(ITelegramBotClient client, Update update)
    {
        long chatId = update.GetChatId();
        Result<int> themeId = update.GetThemeId();

        Task operation = themeId.IsSuccess switch
        {
            true => client.SendMessage(
                chatId: chatId,
                text: _messageOnStart,
                messageThreadId: themeId.Value
            ),
            false => client.SendMessage(chatId: chatId, text: _messageOnStart),
        };

        await operation;
    }
}
