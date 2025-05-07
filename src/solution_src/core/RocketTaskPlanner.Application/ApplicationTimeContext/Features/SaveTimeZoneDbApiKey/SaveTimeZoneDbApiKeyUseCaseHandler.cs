using RocketTaskPlanner.Application.ApplicationTimeContext.Features.Contracts;
using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.ApplicationTimeContext;
using RocketTaskPlanner.Domain.ApplicationTimeContext.ValueObjects;

namespace RocketTaskPlanner.Application.ApplicationTimeContext.Features.SaveTimeZoneDbApiKey;

/// <summary>
/// Создание провайдера временной зоны
/// <param name="repository">Контракт взаимодействия с БД.</param>
/// <param name="idFactory">Фабрика создания Id.</param>
/// <param name="providerFactory">Фабрика создания провайдера временной зоны</param>
/// <typeparam name="TProvider">Тип провайдера временной зоны</typeparam>
/// </summary>
public sealed class SaveTimeZoneDbApiKeyUseCaseHandler<TProvider>(
    IApplicationTimeRepository<TProvider> repository,
    IApplicationTimeProviderIdFactory idFactory,
    IApplicationTimeProviderFactory providerFactory
) : IUseCaseHandler<SaveTimeZoneDbApiKeyUseCase, TProvider>
    where TProvider : IApplicationTimeProvider
{
    /// <summary>
    /// <inheritdoc cref="IApplicationTimeRepository{TProvider}"/>
    /// </summary>
    private readonly IApplicationTimeRepository<TProvider> _repository = repository;

    /// <summary>
    /// <inheritdoc cref="IApplicationTimeProviderIdFactory"/>
    /// </summary>
    private readonly IApplicationTimeProviderIdFactory _idFactory = idFactory;

    /// <summary>
    /// <inheritdoc cref="IApplicationTimeProviderFactory"/>
    /// </summary>
    private readonly IApplicationTimeProviderFactory _providerFactory = providerFactory;

    public async Task<Result<TProvider>> Handle(
        SaveTimeZoneDbApiKeyUseCase useCase,
        CancellationToken ct = default
    )
    {
        string token = useCase.ApiKey;
        IApplicationTimeProviderId id = _idFactory.Create(token);
        TProvider provider = (TProvider)_providerFactory.Create(id);
        Result adding = await _repository.Add(provider, ct);

        return adding.IsFailure ? Result.Failure<TProvider>(adding.Error) : provider;
    }
}
