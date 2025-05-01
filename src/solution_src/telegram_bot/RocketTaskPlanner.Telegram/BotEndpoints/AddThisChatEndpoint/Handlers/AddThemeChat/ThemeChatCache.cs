using PRTelegramBot.Interfaces;

namespace RocketTaskPlanner.Telegram.BotEndpoints.AddThisChatEndpoint.Handlers.AddThemeChat;

public sealed class ThemeChatCache(long chatId, long themeId) : ITelegramCache
{
    private readonly long _chatId = chatId;
    private readonly long _themeId = themeId;
    public long ChatId => _chatId;
    public long ThemeId => _themeId;

    public bool ClearData() => true;
}
