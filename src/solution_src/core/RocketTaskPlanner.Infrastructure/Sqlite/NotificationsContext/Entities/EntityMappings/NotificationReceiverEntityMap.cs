using Dapper.FluentMap.Mapping;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities.EntityMappings;

/// <summary>
/// Маппинг записи таблицы основного чата в Dao модель основного чата при запросах через Dapper
/// </summary>
public sealed class NotificationReceiverEntityMap : EntityMap<NotificationReceiverEntity>
{
    public NotificationReceiverEntityMap()
    {
        Map(r => r.ReceiverId).ToColumn("receiver_id");
        Map(r => r.ReceiverName).ToColumn("receiver_name");
        Map(r => r.ReceiverZoneTimeStamp).ToColumn("receiver_zone_time_stamp");
        Map(r => r.ReceiverZoneName).ToColumn("receiver_zone_name");
    }
}
