using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Infrastructure.Database.ApplicationTimeContext.Cache;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Infrastructure.Database.ApplicationTimeContext.Repositories;

/// <summary>
/// Абстракция работы с хранилищем провайдера временных зон через инстанс кеша
/// </summary>
public sealed class TimeZoneDbCachedRepository : IApplicationTimeRepository<TimeZoneDbProvider>
{
    private readonly IApplicationTimeRepository<TimeZoneDbProvider> _repository;
    private readonly TimeZoneDbProviderCachedInstance _instance;

    public TimeZoneDbCachedRepository(
        IApplicationTimeRepository<TimeZoneDbProvider> repository,
        TimeZoneDbProviderCachedInstance instance)
    {
        _instance = instance;
        _repository = repository;
    }

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
