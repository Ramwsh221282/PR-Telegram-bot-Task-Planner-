using RocketTaskPlanner.Application.NotificationsContext.Visitor;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RegisterTheme;

/// <summary>
/// Бизнес логика добавления темы чата
/// </summary>
/// <param name="ChatId">ID основного чата</param>
/// <param name="ThemeId">ID темы чата</param>
public sealed record RegisterThemeUseCase(long ChatId, long ThemeId) : INotificationVisitableUseCase
{
    public async Task<Result> Accept(
        INotificationUseCaseVisitor visitor,
        CancellationToken ct = default
    ) => await visitor.Visit(this, ct);
}
