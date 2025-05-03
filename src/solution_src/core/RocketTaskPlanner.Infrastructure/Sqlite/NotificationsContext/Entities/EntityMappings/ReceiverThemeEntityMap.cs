using Dapper.FluentMap.Mapping;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities.EntityMappings;

/// <summary>
/// Маппинг записи таблицы темы чата в Dao модель темы чата при запросах через Dapper
/// </summary>
public sealed class ReceiverThemeEntityMap : EntityMap<ReceiverThemeEntity>
{
    public ReceiverThemeEntityMap()
    {
        Map(t => t.ThemeId).ToColumn("theme_id");
        Map(t => t.ReceiverId).ToColumn("receiver_id");
    }
}
