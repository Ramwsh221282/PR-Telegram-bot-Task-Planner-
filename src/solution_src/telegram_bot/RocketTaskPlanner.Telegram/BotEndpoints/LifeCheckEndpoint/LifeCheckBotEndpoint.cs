using CSharpFunctionalExtensions;
using PRTelegramBot.Attributes;
using PRTelegramBot.Extensions;
using PRTelegramBot.Models.Enums;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.LifeCheckEndpoint;

/// <summary>
/// Endpoint для проверки запущен ли бот.
/// </summary>
[BotHandler]
public sealed class LifeCheckBotEndpoint
{
    [ReplyMenuHandler(CommandComparison.Contains, "/ping@", "/ping")]
    public static async Task LifeCheckHandler(ITelegramBotClient client, Update update)
    {
        long chatId = update.GetChatId();
        Result<int> themeId = update.GetThemeId();

        Task reply = themeId.IsSuccess switch
        {
            true => client.SendMessage(
                chatId: chatId,
                text: "pong",
                messageThreadId: themeId.Value
            ),
            false => client.SendMessage(chatId: chatId, text: "pong"),
        };

        await reply;
    }
}
