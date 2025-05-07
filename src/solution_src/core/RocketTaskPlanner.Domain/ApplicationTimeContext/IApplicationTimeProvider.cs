using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Domain.ApplicationTimeContext.ValueObjects;

namespace RocketTaskPlanner.Domain.ApplicationTimeContext;

/// <summary>
/// Абстрактный провайдер временных зон.
/// </summary>
public interface IApplicationTimeProvider
{
    /// <summary>
    /// ID провайдера
    /// </summary>
    IApplicationTimeProviderId Id { get; }

    /// <summary>
    /// Временные зоны
    /// </summary>
    IReadOnlyCollection<ApplicationTimeZone> TimeZones { get; }
}
