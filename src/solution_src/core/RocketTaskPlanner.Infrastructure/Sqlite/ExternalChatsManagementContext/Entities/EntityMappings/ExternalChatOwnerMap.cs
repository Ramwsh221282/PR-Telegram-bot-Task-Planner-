using Dapper.FluentMap.Mapping;

namespace RocketTaskPlanner.Infrastructure.Sqlite.ExternalChatsManagementContext.Entities.EntityMappings;

/// <summary>
/// Маппинг модели таблицы external_chat_owners в <inheritdoc cref="ExternalChatEntity"/>
/// </summary>
public sealed class ExternalChatOwnerMap : EntityMap<ExternalChatOwnerEntity>
{
    public ExternalChatOwnerMap()
    {
        Map(x => x.Id).ToColumn("id");
        Map(x => x.Name).ToColumn("name");
    }
}
