using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Cache;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Repositories;

/// <summary>
/// Абстракция работы с хранилищем провайдера временных зон через инстанс кеша
/// </summary>
/// <param name="repository">Абстракция работы с хранилищем временных зон (БД)</param>
/// <param name="instance">Кешированный инстанс провайдера временных зон</param>
public sealed class TimeZoneDbCachedRepository(
    IApplicationTimeRepository<TimeZoneDbProvider> repository,
    TimeZoneDbProviderCachedInstance instance
) : IApplicationTimeRepository<TimeZoneDbProvider>
{
    private readonly IApplicationTimeRepository<TimeZoneDbProvider> _repository = repository;
    private readonly TimeZoneDbProviderCachedInstance _instance = instance;

    public async Task<Result> Add(TimeZoneDbProvider provider, CancellationToken ct = default)
    {
        Result result = await _repository.Add(provider, ct);
        if (result.IsFailure)
            return result;

        _instance.InitializeOrUpdate(provider);
        return result;
    }

    public async Task<bool> Contains(CancellationToken ct = default)
    {
        if (_instance.Instance != null)
            return true;
        return await _repository.Contains(ct);
    }

    public async Task<Result<TimeZoneDbProvider>> Get(CancellationToken ct = default) =>
        _instance.Instance ?? await _repository.Get(ct);

    public async Task Remove(CancellationToken ct = default)
    {
        await _repository.Remove(ct);
        _instance.SetNull();
    }

    public async Task Save(TimeZoneDbProvider provider, CancellationToken ct = default)
    {
        _instance.InitializeOrUpdate(provider);
        await _repository.Save(provider, ct);
    }
}
