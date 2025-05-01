using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones.ValueObjects;
using RocketTaskPlanner.Domain.ApplicationTimeContext.ValueObjects;

namespace RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;

public sealed class ApplicationTimeZone
{
    public TimeZoneId Id { get; } = null!;
    public IApplicationTimeProviderId ProviderId { get; } = null!;
    public IApplicationTimeProvider Provider { get; } = null!;
    public TimeZoneName Name { get; } = null!;
    public TimeZoneTimeInfo TimeInfo { get; }

    private ApplicationTimeZone() { } // ef core

    public ApplicationTimeZone(
        TimeZoneId id,
        TimeZoneName name,
        TimeZoneTimeInfo info,
        IApplicationTimeProvider provider
    )
    {
        Id = id;
        Name = name;
        TimeInfo = info;
        Provider = provider;
        ProviderId = provider.Id;
    }
}
