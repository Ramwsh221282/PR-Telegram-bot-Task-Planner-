using Dapper.FluentMap.Mapping;

namespace RocketTaskPlanner.Infrastructure.Sqlite.PermissionsContext.Entities.EntityMappings;

public sealed class PermissionEntityMap : EntityMap<PermissionEntity>
{
    public PermissionEntityMap()
    {
        Map(p => p.Id).ToColumn("id");
        Map(p => p.Name).ToColumn("name");
    }
}
