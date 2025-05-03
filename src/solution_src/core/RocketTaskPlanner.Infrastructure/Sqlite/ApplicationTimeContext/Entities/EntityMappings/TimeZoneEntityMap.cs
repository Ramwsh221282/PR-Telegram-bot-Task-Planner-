using Dapper.FluentMap.Mapping;

namespace RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Entities.EntityMappings;

/// <summary>
/// Маппинг модели таблицы из БД в Dao модель временной зоны при запросах через Dapper
/// </summary>
public sealed class TimeZoneEntityMap : EntityMap<TimeZoneEntity>
{
    public TimeZoneEntityMap()
    {
        Map(t => t.TimeZoneId).ToColumn("time_zone_id");
        Map(t => t.ProviderId).ToColumn("provider_id");
        Map(t => t.TimeZoneName).ToColumn("time_zone_name");
        Map(t => t.TimeZoneDateTime).ToColumn("time_zone_date_time");
        Map(t => t.TimeZoneTimeStamp).ToColumn("time_zone_time_stamp");
    }
}
