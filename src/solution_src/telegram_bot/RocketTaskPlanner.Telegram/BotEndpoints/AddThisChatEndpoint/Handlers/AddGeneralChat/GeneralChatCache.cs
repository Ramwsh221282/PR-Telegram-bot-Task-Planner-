using PRTelegramBot.Interfaces;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Telegram.BotEndpoints.AddThisChatEndpoint.Handlers.AddGeneralChat;

public sealed class GeneralChatCache(TimeZoneDbProvider provider, long chatId, string chatName)
    : ITelegramCache
{
    private readonly TimeZoneDbProvider _provider = provider;
    private readonly long _chatId = chatId;
    private readonly string _chatName = chatName;
    public long ChatId => _chatId;
    public string ChatName => _chatName;
    public TimeZoneDbProvider Provider => _provider;

    public bool ClearData() => true;
}
