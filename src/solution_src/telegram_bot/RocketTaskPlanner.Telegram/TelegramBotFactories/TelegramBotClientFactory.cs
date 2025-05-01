using RocketTaskPlanner.Telegram.Configuration;
using Telegram.Bot;

namespace RocketTaskPlanner.Telegram.TelegramBotFactories;

public sealed class TelegramBotClientFactory(BotConfigurationOptions options)
{
    private readonly BotConfigurationOptions _options = options;

    public TelegramBotClient Create() => new(_options.Token);
}
