using Dapper.FluentMap.Mapping;

namespace RocketTaskPlanner.Infrastructure.Sqlite.UsersContext.Entities.EntityMappings;

public sealed class UserEntityMap : EntityMap<UserEntity>
{
    public UserEntityMap()
    {
        Map(u => u.Id).ToColumn("id");
        Map(u => u.Name).ToColumn("name");
    }
}
