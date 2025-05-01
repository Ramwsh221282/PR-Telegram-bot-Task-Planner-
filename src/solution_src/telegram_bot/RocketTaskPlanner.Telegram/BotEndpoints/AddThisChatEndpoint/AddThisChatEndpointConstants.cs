namespace RocketTaskPlanner.Telegram.BotEndpoints.AddThisChatEndpoint;

public static class AddThisChatEndpointConstants
{
    public const string DispatchAddThisChatHandler = "/dispatch";
    public const string GeneralChatHandler = "/generalChat";
    public const string ThemeChatHandler = "/themeChat";
    public const string AddThisChatSelectTimeZoneReply = """
        Для подписывания этого чата Вам необходимо выбрать временную зону.
        Предоставлено меню для выбора временных зон.
        """;

    public const string SelectTimeZoneReply = "Выберите временную зону 👉:";
}
