using RocketTaskPlanner.Telegram.BotEndpoints.ExternalChatsManagementEndpoints.Handlers.AddGeneralChat;

namespace RocketTaskPlanner.Telegram.BotEndpoints.ChangeTimeZoneEndpoint;

/// <summary>
/// Аналогично <inheritdoc cref="AddGeneralChatScope"/>
/// </summary>
public static class ChangeGeneralChatTimeZoneScope
{
    private static readonly Dictionary<
        ChangeChatTimeZoneCitiesEnum,
        ChangeChatTimeZoneScopeInfo
    > _selectors = [];

    /// <summary>
    /// Получение выбранного индекса кнопки из меню временных зон
    /// </summary>
    /// <param name="selector">Перечисление с индексами для меню временных зон</param>
    /// <returns>Информация о выбранной временной зоне</returns>
    public static ChangeChatTimeZoneScopeInfo GetScopeInfo(ChangeChatTimeZoneCitiesEnum selector) =>
        _selectors[selector];

    /// <summary>
    /// Метод для добавления индекса кнопки меню выбора временной зоны
    /// </summary>
    /// <param name="selector">Перечисление с индексами для меню временных зон</param>
    /// <param name="information">Информация о временной зоне</param>
    public static void ManageScopeInfo(
        ChangeChatTimeZoneCitiesEnum selector,
        ChangeChatTimeZoneScopeInfo information
    )
    {
        _selectors.Remove(selector);
        _selectors.Add(selector, information);
    }
}
