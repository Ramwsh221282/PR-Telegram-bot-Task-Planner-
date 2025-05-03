using RocketTaskPlanner.Domain.ApplicationTimeContext;

namespace RocketTaskPlanner.Application.ApplicationTimeContext.Repository;

/// <summary>
/// Контракт взаимодействия с БД в контексте провайдера временной зоны
/// </summary>
/// <typeparam name="TProvider">Тип провайдера</typeparam>
public interface IApplicationTimeRepository<TProvider>
    where TProvider : IApplicationTimeProvider
{
    Task<Result> Add(TProvider provider, CancellationToken ct = default);
    Task<bool> Contains(CancellationToken ct = default);
    Task<Result<TProvider>> Get(CancellationToken ct = default);
    Task<Result> Remove(string? id, CancellationToken ct = default);
    Task Save(TProvider provider, CancellationToken ct = default);
}
