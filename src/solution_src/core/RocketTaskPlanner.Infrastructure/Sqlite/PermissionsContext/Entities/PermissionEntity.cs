using RocketTaskPlanner.Domain.PermissionsContext;

namespace RocketTaskPlanner.Infrastructure.Sqlite.PermissionsContext.Entities;

public sealed class PermissionEntity
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public Permission ConvertToPermission()
    {
        Permission permission = new() { Id = Guid.Parse(Id), Name = Name };
        return permission;
    }
}
