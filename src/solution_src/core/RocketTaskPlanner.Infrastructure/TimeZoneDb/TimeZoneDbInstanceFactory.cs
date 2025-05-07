using RocketTaskPlanner.Application.ApplicationTimeContext.Features.Contracts;
using RocketTaskPlanner.Application.ApplicationTimeContext.Features.SaveTimeZoneDbApiKey;
using RocketTaskPlanner.Domain.ApplicationTimeContext;
using RocketTaskPlanner.Domain.ApplicationTimeContext.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.TimeZoneDb;

/// <summary>
/// Фабрика создания экземпляра <inheritdoc cref="TimeZoneDbProvider"/>
/// </summary>
public sealed class TimeZoneDbInstanceFactory : IApplicationTimeProviderFactory
{
    /// <summary>
    /// Фабричный метод создания <inheritdoc cref="TimeZoneDbProvider"/>
    /// <param name="id">
    ///     <inheritdoc cref="TimeZoneDbToken"/>
    /// </param>
    /// <returns>
    ///     <inheritdoc cref="TimeZoneDbProvider"/>
    /// </returns>
    /// </summary>
    public IApplicationTimeProvider Create(IApplicationTimeProviderId id)
    {
        TimeZoneDbProvider provider = new((TimeZoneDbToken)id);
        return provider;
    }
}
