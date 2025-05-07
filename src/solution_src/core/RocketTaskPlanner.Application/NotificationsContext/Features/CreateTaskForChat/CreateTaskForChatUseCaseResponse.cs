namespace RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChat;

/// <summary>
/// Ответ от выполнения <inheritdoc cref="CreateTaskForChatUseCase"/>
/// </summary>
/// <param name="Information">Сообщение</param>
public sealed record CreateTaskForChatUseCaseResponse(string Information);
