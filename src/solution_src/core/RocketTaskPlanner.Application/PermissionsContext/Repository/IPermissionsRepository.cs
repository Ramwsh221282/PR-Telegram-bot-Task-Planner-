namespace RocketTaskPlanner.Application.PermissionsContext.Repository;

public interface IPermissionsRepository
{
    public IPermissionsReadableRepository ReadableRepository { get; }
    public IPermissionsWritableRepository WritableRepository { get; }
}
