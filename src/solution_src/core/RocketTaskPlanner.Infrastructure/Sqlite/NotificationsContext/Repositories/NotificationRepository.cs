using RocketTaskPlanner.Application.NotificationsContext.Repository;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Repositories;

/// <summary>
/// Фасадный класс объединяющий
/// <inheritdoc cref="INotificationsReadableRepository"/>
/// <inheritdoc cref="INotificationsWritableRepository"/>
/// </summary>
public sealed class NotificationRepository : INotificationRepository
{
    public INotificationsWritableRepository Writable { get; }
    public INotificationsReadableRepository Readable { get; }

    public NotificationRepository(
        INotificationsWritableRepository writable,
        INotificationsReadableRepository readable
    )
    {
        Writable = writable;
        Readable = readable;
    }
}
