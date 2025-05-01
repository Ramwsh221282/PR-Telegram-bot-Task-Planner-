namespace RocketTaskPlanner.Application.NotificationsContext.Features.RegisterTheme;

public sealed record RegisterThemeResponse(long ChatId, string ChatName, long ThemeId)
{
    public string Information() => $"Для чата: {ChatId} {ChatName} подписана тема: {ThemeId}";
}
