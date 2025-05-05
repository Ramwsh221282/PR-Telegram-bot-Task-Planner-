using Dapper.FluentMap.Mapping;

namespace RocketTaskPlanner.Infrastructure.Sqlite.ExternalChatsManagementContext.Entities.EntityMappings;

public sealed class ExternalChatOwnerMap : EntityMap<ExternalChatOwnerEntity>
{
    public ExternalChatOwnerMap()
    {
        Map(x => x.Id).ToColumn("id");
        Map(x => x.Name).ToColumn("name");
    }
}
