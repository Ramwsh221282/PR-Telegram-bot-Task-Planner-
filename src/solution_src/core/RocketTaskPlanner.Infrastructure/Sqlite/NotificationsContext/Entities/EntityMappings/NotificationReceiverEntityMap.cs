using Dapper.FluentMap.Mapping;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities.EntityMappings;

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
