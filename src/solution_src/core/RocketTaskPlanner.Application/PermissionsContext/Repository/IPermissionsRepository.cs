using RocketTaskPlanner.Domain.PermissionsContext;

namespace RocketTaskPlanner.Application.PermissionsContext.Repository;

public interface IPermissionsRepository
{
    Task<Permission> Add(Permission permission, CancellationToken cancellationToken = default);
    Task<Result<Permission>> GetByName(
        string permissionName,
        CancellationToken cancellationToken = default
    );
}
