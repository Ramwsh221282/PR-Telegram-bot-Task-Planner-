namespace RocketTaskPlanner.Telegram.BotEndpoints.AddThisChatEndpoint.Handlers.AddGeneralChat;

public static class AddGeneralChatScope
{
    private static readonly Dictionary<
        AddGeneralChatCitiesEnum,
        AddGeneralChatScopeInfo
    > _selectors = [];

    public static AddGeneralChatScopeInfo GetScopeInfo(AddGeneralChatCitiesEnum selector) =>
        _selectors[selector];

    public static void ManageScopeInfo(
        AddGeneralChatCitiesEnum selector,
        AddGeneralChatScopeInfo information
    )
    {
        _selectors.Remove(selector);
        _selectors.Add(selector, information);
    }
}
