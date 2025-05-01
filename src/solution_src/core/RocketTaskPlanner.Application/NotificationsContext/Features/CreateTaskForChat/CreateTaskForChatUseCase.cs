using RocketTaskPlanner.Application.Shared.UseCaseHandler;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChat;

public sealed record CreateTaskForChatUseCase(
    long ChatId,
    long SubjectId,
    DateTime DateCreated,
    DateTime DateNotify,
    string Message,
    bool isPeriodic
) : IUseCase;
