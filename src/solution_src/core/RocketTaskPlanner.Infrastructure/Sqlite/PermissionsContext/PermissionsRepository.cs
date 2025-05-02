using RocketTaskPlanner.Application.PermissionsContext.Repository;

namespace RocketTaskPlanner.Infrastructure.Sqlite.PermissionsContext;

public sealed class PermissionsRepository : IPermissionsRepository
{
    public IPermissionsReadableRepository ReadableRepository { get; }
    public IPermissionsWritableRepository WritableRepository { get; }

    public PermissionsRepository(
        IPermissionsReadableRepository readableRepository,
        IPermissionsWritableRepository writableRepository
    )
    {
        ReadableRepository = readableRepository;
        WritableRepository = writableRepository;
    }
}
