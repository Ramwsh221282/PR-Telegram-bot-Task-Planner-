using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.ApplicationTimeContext;
using RocketTaskPlanner.Domain.ApplicationTimeContext.ValueObjects;

namespace RocketTaskPlanner.Application.ApplicationTimeContext.Features.SaveTimeZoneDbApiKey;

public sealed class SaveTimeZoneDbApiKeyUseCaseHandler<TProvider>(
    IApplicationTimeRepository<TProvider> repository,
    IApplicationTimeProviderIdFactory idFactory,
    IApplicationTimeProviderFactory providerFactory
) : IUseCaseHandler<SaveTimeZoneDbApiKeyUseCase, TProvider>
    where TProvider : IApplicationTimeProvider
{
    private readonly IApplicationTimeRepository<TProvider> _repository = repository;
    private readonly IApplicationTimeProviderIdFactory _idFactory = idFactory;
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

        return adding.IsFailure
            ? Result.Failure<TProvider>(adding.Error)
            : (Result<TProvider>)provider;
    }
}
