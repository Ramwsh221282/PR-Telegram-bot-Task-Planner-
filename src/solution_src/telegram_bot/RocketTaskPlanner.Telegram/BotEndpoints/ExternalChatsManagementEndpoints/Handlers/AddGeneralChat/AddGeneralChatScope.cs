namespace RocketTaskPlanner.Telegram.BotEndpoints.ExternalChatsManagementEndpoints.Handlers.AddGeneralChat;

public static class AddGeneralChatScope
{
    private static readonly Dictionary<
        AddGeneralChatCitiesEnum,
        AddGeneralChatScopeInfo
    > _selectors = [];

    /// <summary>
    /// Получение выбранного индекса кнопки из меню временных зон
    /// </summary>
    /// <param name="selector">Перечисление с индексами для меню временных зон</param>
    /// <returns>Информация о выбранной временной зоне</returns>
    public static AddGeneralChatScopeInfo GetScopeInfo(AddGeneralChatCitiesEnum selector) =>
        _selectors[selector];

    /// <summary>
    /// Метод для добавления индекса кнопки меню выбора временной зоны
    /// </summary>
    /// <param name="selector">Перечисление с индексами для меню временных зон</param>
    /// <param name="information">Информация о временной зоне</param>
    public static void ManageScopeInfo(
        AddGeneralChatCitiesEnum selector,
        AddGeneralChatScopeInfo information
    )
    {
        _selectors.Remove(selector);
        _selectors.Add(selector, information);
    }
}
