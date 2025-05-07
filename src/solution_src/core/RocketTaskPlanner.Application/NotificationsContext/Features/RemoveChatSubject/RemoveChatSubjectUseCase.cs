using RocketTaskPlanner.Application.NotificationsContext.Visitor;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RemoveChatSubject;

/// <summary>
/// Бизнес логика удаления уведомление чата
/// </summary>
/// <param name="SubjectId">ID уведомления</param>
public sealed record RemoveChatSubjectUseCase(long SubjectId) : INotificationVisitableUseCase
{
    public async Task<Result> Accept(
        INotificationUseCaseVisitor visitor,
        CancellationToken ct = default
    ) => await visitor.Visit(this, ct);
}
