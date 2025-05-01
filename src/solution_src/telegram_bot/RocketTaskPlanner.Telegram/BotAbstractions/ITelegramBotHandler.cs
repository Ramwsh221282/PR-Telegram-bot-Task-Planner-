using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotAbstractions;

public interface ITelegramBotHandler
{
    public string Command { get; }
    public Task Handle(ITelegramBotClient client, Update update);
}
