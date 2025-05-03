using Dapper.FluentMap.Mapping;

namespace RocketTaskPlanner.Infrastructure.Sqlite.UsersContext.Entities.EntityMappings;

/// <summary>
/// Маппинг в Dao права пользователя через запросы Dapper
/// </summary>
public sealed class UserPermissionEntityMap : EntityMap<UserPermissionEntity>
{
    public UserPermissionEntityMap()
    {
        Map(perm => perm.Id).ToColumn("id");
        Map(perm => perm.UserId).ToColumn("user_id");
        Map(perm => perm.Name).ToColumn("name");
    }
}
