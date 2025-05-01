using RocketTaskPlanner.Domain.ApplicationTimeContext;
using RocketTaskPlanner.Domain.ApplicationTimeContext.ValueObjects;

namespace RocketTaskPlanner.Application.ApplicationTimeContext.Features.SaveTimeZoneDbApiKey;

public interface IApplicationTimeProviderFactory
{
    IApplicationTimeProvider Create(IApplicationTimeProviderId id);
}
