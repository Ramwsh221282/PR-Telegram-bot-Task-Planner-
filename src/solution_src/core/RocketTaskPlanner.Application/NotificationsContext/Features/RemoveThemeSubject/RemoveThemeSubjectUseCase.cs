using RocketTaskPlanner.Application.NotificationsContext.Visitor;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RemoveThemeSubject;

/// <summary>
/// Бизнес логика удаления уведомление темы чата
/// </summary>
/// <param name="SubjectId">ID уведомления</param>
public sealed record RemoveThemeSubjectUseCase(long SubjectId) : INotificationVisitableUseCase
{
    public async Task<Result> Accept(
        INotificationUseCaseVisitor visitor,
        CancellationToken ct = default
    ) => await visitor.Visit(this, ct);
}
