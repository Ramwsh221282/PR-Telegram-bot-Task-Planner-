using Dapper.FluentMap.Mapping;

namespace RocketTaskPlanner.Infrastructure.Sqlite.ExternalChatsManagementContext.Entities.EntityMappings;

public sealed class ExternalChatEntityMap : EntityMap<ExternalChatEntity>
{
    public ExternalChatEntityMap()
    {
        Map(x => x.Id).ToColumn("id");
        Map(x => x.Name).ToColumn("name");
        Map(x => x.OwnerId).ToColumn("owner_id");
        Map(x => x.ParentId).ToColumn("parent_id");
    }
}
