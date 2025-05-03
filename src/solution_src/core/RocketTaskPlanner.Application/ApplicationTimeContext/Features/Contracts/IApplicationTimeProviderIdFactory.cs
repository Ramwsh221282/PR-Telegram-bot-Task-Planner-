using RocketTaskPlanner.Domain.ApplicationTimeContext.ValueObjects;

namespace RocketTaskPlanner.Application.ApplicationTimeContext.Features.Contracts;

/// <summary>
/// Фабрика создания Id провайдера временной зоны
/// </summary>
public interface IApplicationTimeProviderIdFactory
{
    /// <summary>
    /// ID провайдера временной зоны
    /// </summary>
    /// <param name="token">Токен</param>
    /// <returns>Id провайдера временной зоны</returns>
    IApplicationTimeProviderId Create(string token);
}
