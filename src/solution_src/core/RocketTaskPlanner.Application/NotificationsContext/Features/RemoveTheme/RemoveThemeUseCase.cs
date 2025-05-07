using RocketTaskPlanner.Application.NotificationsContext.Visitor;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RemoveTheme;

/// <summary>
/// Бизнес логика удаления тему чата для уведомлений
/// </summary>
/// <param name="ChatId">ID чата</param>
/// <param name="ThemeId">ID темы</param>
public sealed record RemoveThemeUseCase(long ChatId, long ThemeId) : INotificationVisitableUseCase
{
    public async Task<Result> Accept(
        INotificationUseCaseVisitor visitor,
        CancellationToken ct = default
    ) => await visitor.Visit(this, ct);
}
