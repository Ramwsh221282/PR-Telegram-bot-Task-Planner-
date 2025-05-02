using RocketTaskPlanner.Domain.PermissionsContext;

namespace RocketTaskPlanner.Application.PermissionsContext.Repository;

public interface IPermissionsReadableRepository
{
    Task<Result<Permission>> GetByName(string permissionName, CancellationToken ct = default);
    Task<bool> Contains(string permissionName, CancellationToken ct = default);
}
