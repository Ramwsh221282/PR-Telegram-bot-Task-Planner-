using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones.ValueObjects;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Entities;

public sealed class TimeZoneEntity
{
    public string TimeZoneId { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public string TimeZoneName { get; set; } = string.Empty;
    public DateTime TimeZoneDateTime { get; set; } = DateTime.MinValue;
    public long TimeZoneTimeStamp { get; set; }

    public ApplicationTimeZone ToTimeZone(TimeZoneDbProvider provider)
    {
        TimeZoneId id = Domain
            .ApplicationTimeContext.Entities.TimeZones.ValueObjects.TimeZoneId.Create(TimeZoneId)
            .Value;
        TimeZoneName name = Domain
            .ApplicationTimeContext.Entities.TimeZones.ValueObjects.TimeZoneName.Create(
                TimeZoneName
            )
            .Value;
        TimeZoneTimeInfo timeInfo = TimeZoneTimeInfo.Create(TimeZoneTimeStamp).Value;
        ApplicationTimeZone zone = new ApplicationTimeZone(id, name, timeInfo, provider);
        return zone;
    }
}
