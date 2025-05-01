using RocketTaskPlanner.Application.ApplicationTimeContext.Features.SaveTimeZoneDbApiKey;
using RocketTaskPlanner.Domain.ApplicationTimeContext.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.TimeZoneDb;

public sealed class TimeZoneDbTokenFactory : IApplicationTimeProviderIdFactory
{
    public IApplicationTimeProviderId Create(string token)
    {
        return TimeZoneDbToken.Create(token).Value;
    }
}
