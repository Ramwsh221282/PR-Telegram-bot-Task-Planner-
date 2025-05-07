using RocketTaskPlanner.Telegram.Configuration;
using Telegram.Bot;

namespace RocketTaskPlanner.Telegram.TelegramBotFactories;

/// <summary>
/// Фабрика создания экземпляра telegram bot клиента
/// </summary>
/// <param name="options">
///     <inheritdoc cref="BotConfigurationOptions"/>
/// </param>
public sealed class TelegramBotClientFactory(BotConfigurationOptions options)
{
    /// <summary>
    ///     <inheritdoc cref="BotConfigurationOptions"/>
    /// </summary>
    private readonly BotConfigurationOptions _options = options;

    /// <summary>
    /// Фабричный метод создания Telegram Bot клиента
    /// </summary>
    /// <returns>
    ///     <inheritdoc cref="TelegramBotClient"/>
    /// </returns>
    public TelegramBotClient Create() => new(_options.Token);
}
