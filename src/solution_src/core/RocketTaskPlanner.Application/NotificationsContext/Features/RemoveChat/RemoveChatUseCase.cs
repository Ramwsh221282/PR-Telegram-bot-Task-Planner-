using RocketTaskPlanner.Application.NotificationsContext.Visitor;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RemoveChat;

/// <summary>
/// Бизнес логика удаления чата для уведомлений
/// </summary>
/// <param name="chatId">ID чата для уведомлений</param>
public sealed record RemoveChatUseCase(long chatId) : INotificationVisitableUseCase
{
    public async Task<Result> Accept(
        INotificationUseCaseVisitor visitor,
        CancellationToken ct = default
    ) => await visitor.Visit(this, ct);
}
