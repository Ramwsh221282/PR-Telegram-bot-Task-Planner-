namespace RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChatTheme;

/// <summary>
/// Ответ от выполнения команды <inheritdoc cref="CreateTaskForChatThemeUseCase"/>
/// </summary>
/// <param name="Information">Сообщение</param>
public sealed record CreateTaskForChatThemeUseCaseResponse(string Information);
