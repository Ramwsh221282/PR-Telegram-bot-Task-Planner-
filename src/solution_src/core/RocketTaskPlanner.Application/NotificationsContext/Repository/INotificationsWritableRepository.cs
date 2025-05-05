using RocketTaskPlanner.Application.Shared.UnitOfWorks;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;

namespace RocketTaskPlanner.Application.NotificationsContext.Repository;

/// <summary>
/// Контракт взаимодействия с БД в контексте чатов и уведомлений
/// </summary>
public interface INotificationsWritableRepository : IRepository
{
    Task<Result> Add(
        NotificationReceiver receiver,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    );
    Result AddTheme(ReceiverTheme theme, IUnitOfWork unitOfWork, CancellationToken ct = default);
    Task<Result> AddSubject(ThemeChatSubject subject, CancellationToken ct = default);
    Task<Result> AddSubject(GeneralChatReceiverSubject subject, CancellationToken ct = default);
    Result Remove(long? id, IUnitOfWork unitOfWork, CancellationToken ct = default);
    Result RemoveTheme(ReceiverTheme theme, IUnitOfWork unit, CancellationToken ct = default);
}
