namespace RocketTaskPlanner.Application.NotificationsContext.Features.RegisterChat;

/// <summary>
/// Ответ от выполнения <inheritdoc cref="RegisterChatUseCase"/>
/// </summary>
/// <param name="RegisteredChatId">ID добавленного чата</param>
/// <param name="RegisteredChatName">Название добавленного чата</param>
public sealed record RegisterChatUseCaseResponse(long RegisteredChatId, string RegisteredChatName)
{
    public string Information() => $"Чат: {RegisteredChatName} с ID: {RegisteredChatId} подписан.";
}
