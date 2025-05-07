using RocketTaskPlanner.Application.NotificationsContext.Visitor;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RegisterChat;

/// <summary>
/// Бизнес логика создания чата для уведомлений
/// </summary>
/// <param name="ChatId">ID чата</param>
/// <param name="ChatName">Название чата</param>
/// <param name="ZoneName">Название временной зоны</param>
public sealed record RegisterChatUseCase(long ChatId, string ChatName, string ZoneName)
    : INotificationVisitableUseCase
{
    public async Task<Result> Accept(
        INotificationUseCaseVisitor visitor,
        CancellationToken ct = default
    ) => await visitor.Visit(this, ct);
}
