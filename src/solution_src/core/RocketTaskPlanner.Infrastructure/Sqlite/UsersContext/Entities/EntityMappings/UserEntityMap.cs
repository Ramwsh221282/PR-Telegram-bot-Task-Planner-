using Dapper.FluentMap.Mapping;

namespace RocketTaskPlanner.Infrastructure.Sqlite.UsersContext.Entities.EntityMappings;

/// <summary>
/// Маппинг в Dao пользователя из БД при запросах через Dapper.
/// </summary>
public sealed class UserEntityMap : EntityMap<UserEntity>
{
    public UserEntityMap()
    {
        Map(u => u.Id).ToColumn("id");
        Map(u => u.Name).ToColumn("name");
    }
}
