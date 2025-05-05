namespace RocketTaskPlanner.Application.NotificationsContext.Repository;

public interface INotificationRepository
{
    public INotificationsWritableRepository Writable { get; }
    public INotificationsReadableRepository Readable { get; }
}
