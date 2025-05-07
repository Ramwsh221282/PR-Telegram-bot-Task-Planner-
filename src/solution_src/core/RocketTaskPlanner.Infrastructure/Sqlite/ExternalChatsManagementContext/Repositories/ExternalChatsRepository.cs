using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;

namespace RocketTaskPlanner.Infrastructure.Sqlite.ExternalChatsManagementContext.Repositories;

/// <summary>
/// Фасадный класс для объединения
/// <inheritdoc cref="IExternalChatsReadableRepository"/>
/// <inheritdoc cref="IExternalChatsWritableRepository"/>
/// </summary>
public sealed class ExternalChatsRepository
    : Application.ExternalChatsManagementContext.Repository.IExternalChatsRepository
{
    public IExternalChatsWritableRepository Writable { get; }
    public IExternalChatsReadableRepository Readable { get; }

    public ExternalChatsRepository(
        IExternalChatsWritableRepository writable,
        IExternalChatsReadableRepository readable
    )
    {
        Writable = writable;
        Readable = readable;
    }
}
