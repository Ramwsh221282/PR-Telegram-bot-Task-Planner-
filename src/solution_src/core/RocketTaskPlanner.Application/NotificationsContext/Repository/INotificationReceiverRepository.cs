using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;

namespace RocketTaskPlanner.Application.NotificationsContext.Repository;

public interface INotificationReceiverRepository
{
    public Task<Result<NotificationReceiver>> GetById(long? id, CancellationToken ct = default);
    public Task<Result> Add(NotificationReceiver receiver, CancellationToken ct = default);
    public Task<Result> AddTheme(ReceiverTheme theme, CancellationToken ct = default);
    public Task<Result> AddSubject(ThemeChatSubject subject, CancellationToken ct = default);
    public Task<Result> AddSubject(
        GeneralChatReceiverSubject subject,
        CancellationToken ct = default
    );
    public Task<Result> Remove(long? id, CancellationToken ct = default);
}
