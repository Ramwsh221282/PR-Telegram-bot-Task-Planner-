using RocketTaskPlanner.Application.NotificationsContext.Visitor;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChatTheme;

/// <summary>
/// Команда для бизнес логики создания уведомления темы чата
/// </summary>
/// <param name="ChatId">ID основного чата</param>
/// <param name="ThemeId">ID темы чата</param>
/// <param name="SubjectId">ID уведомления</param>
/// <param name="DateCreated">Дата создания</param>
/// <param name="DateNotify">Дата уведомления</param>
/// <param name="Message">Текст уведомления</param>
/// <param name="isPeriodic">Пероидоическая или нет</param>
public record CreateTaskForChatThemeUseCase(
    long ChatId,
    long ThemeId,
    long SubjectId,
    DateTime DateCreated,
    DateTime DateNotify,
    string Message,
    bool isPeriodic
) : INotificationVisitableUseCase
{
    public async Task<Result> Accept(
        INotificationUseCaseVisitor visitor,
        CancellationToken ct = default
    ) => await visitor.Visit(this, ct);
}
