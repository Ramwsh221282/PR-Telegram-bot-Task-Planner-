using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Infrastructure.Database.ApplicationTimeContext.Entities;

/// <summary>
/// Dao модель провайдера временной зоны
/// </summary>
public sealed class TimeZoneDbProviderEntity
{
    public string TimeZoneDbProviderId { get; set; } = string.Empty;
    public List<TimeZoneEntity> TimeZones { get; set; } = [];

    public TimeZoneDbProvider ToTimeZoneDbProvider()
    {
        TimeZoneDbToken token = TimeZoneDbToken.Create(TimeZoneDbProviderId).Value;
        TimeZoneDbProvider provider = new TimeZoneDbProvider(token);
        List<ApplicationTimeZone> zones = [.. TimeZones.Select(t => t.ToTimeZone(provider))];
        return new TimeZoneDbProvider(token, zones);
    }
}
