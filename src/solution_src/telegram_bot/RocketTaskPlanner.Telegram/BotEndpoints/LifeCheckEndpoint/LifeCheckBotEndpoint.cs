using PRTelegramBot.Attributes;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.LifeCheckEndpoint;

[BotHandler]
public sealed class LifeCheckBotEndpoint
{
    [ReplyMenuHandler("/ping")]
    public static async Task LifeCheckHandler(ITelegramBotClient client, Update update)
    {
        await PRTelegramBot.Helpers.Message.Send(client, update, "pong");
    }
}
