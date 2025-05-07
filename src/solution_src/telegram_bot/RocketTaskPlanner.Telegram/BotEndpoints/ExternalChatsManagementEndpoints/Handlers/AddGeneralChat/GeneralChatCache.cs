using PRTelegramBot.Interfaces;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Telegram.BotEndpoints.ExternalChatsManagementEndpoints.Handlers.AddGeneralChat;

/// <summary>
/// Кеш для хранения данных команды /add_this_chat
/// </summary>
/// <param name="provider">Инстанс провайдера временных зон</param>
/// <param name="chatId">Ид чата</param>
/// <param name="chatName">Название чата</param>
public sealed class GeneralChatCache(TimeZoneDbProvider provider, long chatId, string chatName)
    : ITelegramCache
{
    /// <summary>
    /// ID чата
    /// </summary>
    public long ChatId { get; } = chatId;

    /// <summary>
    /// Название чата
    /// </summary>
    public string ChatName { get; } = chatName;

    /// <summary>
    /// <inheritdoc cref="TimeZoneDbProvider"/>
    /// </summary>
    public TimeZoneDbProvider Provider { get; } = provider;

    /// <summary>
    /// Заглушка очистки кеша
    /// </summary>
    public bool ClearData() => true;
}
