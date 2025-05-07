using RocketTaskPlanner.Application.NotificationsContext.Visitor;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.ChangeTimeZone;

/// <summary>
/// Команда для вызова бизнес логики изменения временной зоны у чата.
/// </summary>
/// <param name="ChatId">ID чата</param>
/// <param name="ZoneName">Название временной зоны</param>
public sealed record ChangeTimeZoneUseCase(long ChatId, string ZoneName)
    : INotificationVisitableUseCase
{
    public async Task<Result> Accept(
        INotificationUseCaseVisitor visitor,
        CancellationToken ct = default
    ) => await visitor.Visit(this, ct);
}
