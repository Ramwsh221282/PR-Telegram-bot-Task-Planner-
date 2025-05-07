using RocketTaskPlanner.Application.ApplicationTimeContext.Features.Contracts;
using RocketTaskPlanner.Application.ApplicationTimeContext.Features.SaveTimeZoneDbApiKey;
using RocketTaskPlanner.Domain.ApplicationTimeContext.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.TimeZoneDb;

/// <summary>
/// Фабрика создания ID <inheritdoc cref="IApplicationTimeProviderId"/>
/// </summary>
public sealed class TimeZoneDbTokenFactory : IApplicationTimeProviderIdFactory
{
    /// <summary>
    /// Фабричный метод создания
    ///     <inheritdoc cref="IApplicationTimeProviderId"/>
    /// </summary>
    /// <param name="token">
    ///     Токен
    /// </param>
    /// <returns>
    ///     <inheritdoc cref="TimeZoneDbTokenFactory"/>
    /// </returns>
    public IApplicationTimeProviderId Create(string token)
    {
        return TimeZoneDbToken.Create(token).Value;
    }
}
