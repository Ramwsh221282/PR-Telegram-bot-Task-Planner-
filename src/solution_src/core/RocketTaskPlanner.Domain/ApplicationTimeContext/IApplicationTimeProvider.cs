using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Domain.ApplicationTimeContext.ValueObjects;

namespace RocketTaskPlanner.Domain.ApplicationTimeContext;

public interface IApplicationTimeProvider
{
    IApplicationTimeProviderId Id { get; }
    IReadOnlyCollection<ApplicationTimeZone> TimeZones { get; }
}
