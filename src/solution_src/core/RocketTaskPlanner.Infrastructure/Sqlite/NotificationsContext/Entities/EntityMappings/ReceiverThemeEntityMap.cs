using Dapper.FluentMap.Mapping;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities.EntityMappings;

public sealed class ReceiverThemeEntityMap : EntityMap<ReceiverThemeEntity>
{
    public ReceiverThemeEntityMap()
    {
        Map(t => t.ThemeId).ToColumn("theme_id");
        Map(t => t.ReceiverId).ToColumn("receiver_id");
    }
}
