namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;

public interface IExternalChatsRepository
{
    public IExternalChatsWritableRepository Writable { get; }
    public IExternalChatsReadableRepository Readable { get; }
}
