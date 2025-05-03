using RocketTaskPlanner.Telegram.Configuration;
using Telegram.Bot;

namespace RocketTaskPlanner.Telegram.TelegramBotFactories;

/// <summary>
/// Фабрика создания экземпляра telegram bot клиента
/// </summary>
/// <param name="options">Настройки telegram бот с токеном бота из BotFather</param>
public sealed class TelegramBotClientFactory(BotConfigurationOptions options)
{
    private readonly BotConfigurationOptions _options = options;

    public TelegramBotClient Create() => new(_options.Token);
}
