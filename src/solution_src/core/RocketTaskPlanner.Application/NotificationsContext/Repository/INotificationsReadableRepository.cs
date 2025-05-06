using RocketTaskPlanner.Application.Shared.UnitOfWorks;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

namespace RocketTaskPlanner.Application.NotificationsContext.Repository;

public interface INotificationsReadableRepository : IRepository
{
    Task<Result<NotificationReceiver>> GetById(long? id, CancellationToken ct = default);
    Task<Result<NotificationReceiverName>> GetNameById(long? id, CancellationToken ct = default);
}
