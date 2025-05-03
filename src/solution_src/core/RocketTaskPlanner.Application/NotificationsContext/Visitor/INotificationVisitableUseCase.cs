using RocketTaskPlanner.Application.Shared.UseCaseHandler;

namespace RocketTaskPlanner.Application.NotificationsContext.Visitor;

/// <summary>
/// Интерфейс запроса бизнес логики в контексте уведомлений и чатов
/// </summary>
public interface INotificationVisitableUseCase : IUseCase
{
    Task<Result> Accept(INotificationUseCaseVisitor visitor, CancellationToken ct = default);
}
