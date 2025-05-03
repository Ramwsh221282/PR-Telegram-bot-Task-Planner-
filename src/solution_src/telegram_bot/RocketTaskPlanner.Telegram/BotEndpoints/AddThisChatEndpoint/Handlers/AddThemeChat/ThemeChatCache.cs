using PRTelegramBot.Interfaces;

namespace RocketTaskPlanner.Telegram.BotEndpoints.AddThisChatEndpoint.Handlers.AddThemeChat;

/// <summary>
/// Кешированная информация при добавлении темы чата
/// </summary>
/// <param name="chatId">Id чата</param>
/// <param name="themeId">Id темы</param>
public sealed class ThemeChatCache(long chatId, long themeId) : ITelegramCache
{
    public long ChatId { get; } = chatId;
    public long ThemeId { get; } = themeId;

    public bool ClearData() => true;
}
