using RocketTaskPlanner.Domain.ApplicationTimeContext;
using RocketTaskPlanner.Domain.ApplicationTimeContext.ValueObjects;

namespace RocketTaskPlanner.Application.ApplicationTimeContext.Features.Contracts;

/// <summary>
/// Фабрика создания экземпляра провайдера временной зоны
/// </summary>
public interface IApplicationTimeProviderFactory
{
    /// <summary>
    /// Создание экземпляра провайдера временной зоны
    /// </summary>
    /// <param name="id">Id провайдера</param>
    /// <returns>Провайдер временной зоны</returns>
    IApplicationTimeProvider Create(IApplicationTimeProviderId id);
}
