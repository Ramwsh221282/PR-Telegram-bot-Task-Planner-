using RocketTaskPlanner.Application.ApplicationTimeContext.Features.SaveTimeZoneDbApiKey;
using RocketTaskPlanner.Domain.ApplicationTimeContext;
using RocketTaskPlanner.Domain.ApplicationTimeContext.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.TimeZoneDb;

public sealed class TimeZoneDbInstanceFactory : IApplicationTimeProviderFactory
{
    public IApplicationTimeProvider Create(IApplicationTimeProviderId id)
    {
        TimeZoneDbProvider provider = new((TimeZoneDbToken)id);
        return provider;
    }
}
