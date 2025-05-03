using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones.ValueObjects;
using RocketTaskPlanner.Domain.ApplicationTimeContext.ValueObjects;

namespace RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;

/// <summary>
/// Временная зона
/// </summary>
public sealed class ApplicationTimeZone
{
    public TimeZoneId Id { get; } = null!;
    public IApplicationTimeProviderId ProviderId { get; } = null!;

    /// <summary>
    /// Провайдер-обладатель временной зоны
    /// </summary>
    public IApplicationTimeProvider Provider { get; } = null!;

    /// <summary>
    /// Название временной зоны
    /// </summary>
    public TimeZoneName Name { get; } = null!;

    /// <summary>
    /// Информация о времени временной зоны
    /// </summary>
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
