using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.ApplicationTimeContext;

namespace RocketTaskPlanner.Application.ApplicationTimeContext.Features.DropTimeZoneDbApiKey;

/// <summary>
/// Обработчик удаления провайдера временной зоны.
/// <param name="repository">Контракт взаимодействия с БД.</param>
/// <typeparam name="TProvider">Тип провайдера временной зоны</typeparam>
/// </summary>
public sealed class DropTimeZoneDbApiKeyUseCaseHandler<TProvider>(
    IApplicationTimeRepository<TProvider> repository
) : IUseCaseHandler<DropTimeZoneDbApiKeyUseCase, string>
    where TProvider : IApplicationTimeProvider
{
    /// <summary>
    /// <inheritdoc cref="IApplicationTimeRepository{TProvider}"/>
    /// </summary>
    private readonly IApplicationTimeRepository<TProvider> _repository = repository;

    public async Task<Result<string>> Handle(
        DropTimeZoneDbApiKeyUseCase useCase,
        CancellationToken ct = default
    )
    {
        await _repository.Remove(ct);
        return Result.Success<string>("OK");
    }
}
