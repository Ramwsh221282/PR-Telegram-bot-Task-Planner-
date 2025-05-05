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
    public long ChatId { get; } = chatId;

    public string ChatName { get; } = chatName;

    public TimeZoneDbProvider Provider { get; } = provider;

    public bool ClearData() => true;
}
