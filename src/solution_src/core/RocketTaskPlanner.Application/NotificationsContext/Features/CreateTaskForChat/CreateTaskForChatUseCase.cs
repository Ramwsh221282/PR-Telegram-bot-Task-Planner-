using RocketTaskPlanner.Application.NotificationsContext.Visitor;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChat;

public sealed record CreateTaskForChatUseCase(
    long ChatId,
    long SubjectId,
    DateTime DateCreated,
    DateTime DateNotify,
    string Message,
    bool isPeriodic
) : INotificationVisitableUseCase
{
    public async Task<Result> Accept(
        INotificationUseCaseVisitor visitor,
        CancellationToken ct = default
    ) => await visitor.Visit(this, ct);
}
