using RocketTaskPlanner.Application.NotificationsContext.Visitor;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChat;

/// <summary>
/// Команда для бизнес логики создания уведомления для основного чата
/// </summary>
/// <param name="ChatId">ID чата</param>
/// <param name="SubjectId">ID уведомления</param>
/// <param name="DateCreated">Дата создания</param>
/// <param name="DateNotify">Дата уведомления</param>
/// <param name="Message">Текст уведомления</param>
/// <param name="isPeriodic">Периодичная или нет</param>
public sealed record CreateTaskForChatUseCase(
    long ChatId,
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
