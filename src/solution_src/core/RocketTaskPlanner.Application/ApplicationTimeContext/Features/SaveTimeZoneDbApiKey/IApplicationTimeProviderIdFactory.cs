using RocketTaskPlanner.Domain.ApplicationTimeContext.ValueObjects;

namespace RocketTaskPlanner.Application.ApplicationTimeContext.Features.SaveTimeZoneDbApiKey;

public interface IApplicationTimeProviderIdFactory
{
    IApplicationTimeProviderId Create(string token);
}
