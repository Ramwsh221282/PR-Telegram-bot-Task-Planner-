using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;

namespace RocketTaskPlanner.Application.NotificationsContext.Repository;

/// <summary>
/// Контракт взаимодействия с БД в контексте чатов и уведомлений
/// </summary>
public interface INotificationReceiverWritableRepository
{
    Task<Result> Add(NotificationReceiver receiver, CancellationToken ct = default);
    Task<Result> AddTheme(ReceiverTheme theme, CancellationToken ct = default);
    Task<Result> AddSubject(ThemeChatSubject subject, CancellationToken ct = default);
    Task<Result> AddSubject(GeneralChatReceiverSubject subject, CancellationToken ct = default);
    public Task<Result> Remove(long? id, CancellationToken ct = default);
    public Task<Result<NotificationReceiver>> GetById(long? id, CancellationToken ct = default);
}
