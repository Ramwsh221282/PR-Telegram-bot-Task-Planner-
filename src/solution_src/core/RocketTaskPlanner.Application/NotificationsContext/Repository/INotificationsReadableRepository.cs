using RocketTaskPlanner.Domain.NotificationsContext;

namespace RocketTaskPlanner.Application.NotificationsContext.Repository;

public interface INotificationsReadableRepository
{
    Task<Result<NotificationReceiver>> GetById(long? id, CancellationToken ct = default);
}
