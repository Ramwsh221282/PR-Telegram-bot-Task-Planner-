using RocketTaskPlanner.Domain.PermissionsContext;

namespace RocketTaskPlanner.Application.PermissionsContext.Repository;

/// <summary>
/// Контракт для взаимодействия с БД (запись)
/// </summary>
public interface IPermissionsWritableRepository
{
    Task<Permission> Add(Permission permission, CancellationToken cancellationToken = default);
}
