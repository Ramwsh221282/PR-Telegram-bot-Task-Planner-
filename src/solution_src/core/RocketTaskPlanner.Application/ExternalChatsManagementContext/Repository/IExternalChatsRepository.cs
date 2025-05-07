namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;

/// <summary>
/// Контракт фасада объединяющий
/// <inheritdoc cref="IExternalChatsReadableRepository"/>
/// и
/// <inheritdoc cref="IExternalChatsWritableRepository"/>
/// </summary>
public interface IExternalChatsRepository
{
    public IExternalChatsWritableRepository Writable { get; }
    public IExternalChatsReadableRepository Readable { get; }
}
