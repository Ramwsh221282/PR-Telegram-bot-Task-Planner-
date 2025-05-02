using RocketTaskPlanner.Application.UsersContext.Contracts;

namespace RocketTaskPlanner.Infrastructure.Sqlite.UsersContext;

public sealed class UsersRepository : IUsersRepository
{
    public IUsersWritableRepository WritableRepository { get; }
    public IUsersReadableRepository ReadableRepository { get; }

    public UsersRepository(
        IUsersWritableRepository writableRepository,
        IUsersReadableRepository readableRepository
    )
    {
        WritableRepository = writableRepository;
        ReadableRepository = readableRepository;
    }
}
