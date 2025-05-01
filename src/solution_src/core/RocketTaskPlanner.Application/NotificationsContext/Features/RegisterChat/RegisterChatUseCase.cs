using RocketTaskPlanner.Application.Shared.UseCaseHandler;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RegisterChat;

public sealed record RegisterChatUseCase(
    long ChatId,
    string ChatName,
    string ZoneName,
    long TimeStamp
) : IUseCase;
