using RocketTaskPlanner.Domain.PermissionsContext;

namespace RocketTaskPlanner.Application.PermissionsContext.Repository;

public interface IPermissionsWritableRepository
{
    Task<Permission> Add(Permission permission, CancellationToken cancellationToken = default);
}
