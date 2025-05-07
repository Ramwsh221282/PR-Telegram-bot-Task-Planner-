namespace RocketTaskPlanner.Application.NotificationsContext.Features.RegisterTheme;

/// <summary>
/// Ответ от <inheritdoc cref="RegisterThemeUseCase"/>
/// </summary>
/// <param name="ChatId">ID основного чата</param>
/// <param name="ChatName">Название основного чата</param>
/// <param name="ThemeId">ID темы чата</param>
public sealed record RegisterThemeResponse(long ChatId, string ChatName, long ThemeId)
{
    public string Information() => $"Для чата: {ChatId} {ChatName} подписана тема: {ThemeId}";
}
