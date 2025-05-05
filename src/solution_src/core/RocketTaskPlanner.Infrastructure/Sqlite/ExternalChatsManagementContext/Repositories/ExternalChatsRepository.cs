using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;

namespace RocketTaskPlanner.Infrastructure.Sqlite.ExternalChatsManagementContext.Repositories;

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
