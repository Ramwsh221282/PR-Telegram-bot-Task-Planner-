using RocketTaskPlanner.Application.Shared.UseCaseHandler;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RegisterTheme;

public sealed record RegisterThemeUseCase(long ChatId, long ThemeId) : IUseCase;
