using RocketTaskPlanner.Application.Shared.UseCaseHandler;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChatTheme;

public record CreateTaskForChatThemeUseCase(
    long ChatId,
    long ThemeId,
    long SubjectId,
    DateTime DateCreated,
    DateTime DateNotify,
    string Message,
    bool isPeriodic
) : IUseCase;
