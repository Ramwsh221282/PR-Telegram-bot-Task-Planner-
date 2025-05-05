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
/// Endpoint обработок для команды /start
/// Здесь происходит конфигурирование Time Zone Db ключа.
/// </summary>
[BotHandler]
public sealed class BotStartEndpoint
{
    /// <summary>
    /// Точка входа в команду /start
    /// </summary>
    /// <param name="client">Сконфигурированный телеграм-бот клиент.</param>
    /// <param name="update">Событие о пользовательском вводе.</param>
    [ReplyMenuHandler(CommandComparison.Contains, StringComparison.OrdinalIgnoreCase, ["/start"])]
    public async Task OnStart(ITelegramBotClient client, Update update)
    {
        long chatId = update.GetChatId();
        Result<int> themeId = update.GetThemeId();
        Task operation = themeId.IsSuccess switch
        {
            true => client.SendMessage(
                chatId: chatId,
                text: "TODO",
                messageThreadId: themeId.Value
            ),
            false => client.SendMessage(chatId: chatId, text: "TODO"),
        };
        await operation;
    }
}
