namespace RocketTaskPlanner.Application.NotificationsContext.Repository;

/// <summary>
/// Контракт для фасада контрактов
/// <inheritdoc cref="INotificationsWritableRepository"/>
/// и
/// <inheritdoc cref="INotificationsReadableRepository"/>
/// </summary>
public interface INotificationRepository
{
    public INotificationsWritableRepository Writable { get; }
    public INotificationsReadableRepository Readable { get; }
}
