using PRTelegramBot.Interfaces;

namespace RocketTaskPlanner.Telegram.BotEndpoints.ExternalChatsManagementEndpoints.Handlers.AddThemeChat;

/// <summary>
/// Кешированная информация при добавлении темы чата
/// </summary>
/// <param name="chatId">Id чата</param>
/// <param name="themeId">Id темы</param>
public sealed class ThemeChatCache(long chatId, long themeId, long userId, string chatName)
    : ITelegramCache
{
    public long ChatId { get; } = chatId;
    public long ThemeId { get; } = themeId;

    public long UserId { get; } = userId;

    public string ChatName { get; } = chatName;

    public bool ClearData() => true;
}
