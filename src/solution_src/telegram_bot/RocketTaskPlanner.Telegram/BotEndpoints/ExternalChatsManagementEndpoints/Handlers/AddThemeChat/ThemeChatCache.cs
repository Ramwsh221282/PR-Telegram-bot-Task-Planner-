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
    /// <summary>
    /// Id основного чата
    /// </summary>
    public long ChatId { get; } = chatId;

    /// <summary>
    /// Id темы чата
    /// </summary>
    public long ThemeId { get; } = themeId;

    /// <summary>
    /// Id обладателя основного чата
    /// </summary>
    public long UserId { get; } = userId;

    /// <summary>
    /// Название чата (заголовок)
    /// </summary>
    public string ChatName { get; } = chatName;

    /// <summary>
    /// Заглушка очистки кеша
    /// </summary>
    public bool ClearData() => true;
}
