namespace RocketTaskPlanner.Telegram.BotEndpoints.ExternalChatsManagementEndpoints;

/// <summary>
/// Константы для отправки reply сообщений и хранения имён хендлеров в контексте endpoint добавления чата в бота
/// </summary>
public static class AddThisChatEndpointConstants
{
    public const string DispatchAddThisChatHandler = "/dispatch";
    public const string ThemeChatHandler = "/themeChat";
    public const string AddThisChatSelectTimeZoneReply = """
        Для подписывания этого чата Вам необходимо выбрать временную зону.
        Предоставлено меню для выбора временных зон.
        """;

    public const string SelectTimeZoneReply = "Выберите временную зону 👉:";
}
