using Dapper.FluentMap.Mapping;

namespace RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Entities.EntityMappings;

public sealed class TimeZoneDbProviderEntityMap : EntityMap<TimeZoneDbProviderEntity>
{
    public TimeZoneDbProviderEntityMap()
    {
        Map(p => p.TimeZoneDbProviderId).ToColumn("time_zone_db_provider_id");
    }
}
