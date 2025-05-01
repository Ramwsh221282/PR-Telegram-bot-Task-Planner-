namespace RocketTaskPlanner.Application.NotificationsContext.Features.RegisterChat;

public sealed record RegisterChatUseCaseResponse(long RegisteredChatId, string RegisteredChatName)
{
    public string Information() => $"Чат: {RegisteredChatName} с ID: {RegisteredChatId} подписан.";
}
