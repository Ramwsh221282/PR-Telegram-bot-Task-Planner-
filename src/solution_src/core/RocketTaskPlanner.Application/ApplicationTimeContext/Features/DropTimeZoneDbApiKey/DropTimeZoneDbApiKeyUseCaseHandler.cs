using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.ApplicationTimeContext;

namespace RocketTaskPlanner.Application.ApplicationTimeContext.Features.DropTimeZoneDbApiKey;

/// <summary>
/// Обработчик удаления провайдера временной зоны.
/// </summary>
/// <param name="repository">Контракт взаимодействия с БД.</param>
/// <typeparam name="TProvider">Тип провайдера временной зоны</typeparam>
public sealed class DropTimeZoneDbApiKeyUseCaseHandler<TProvider>(
    IApplicationTimeRepository<TProvider> repository
) : IUseCaseHandler<DropTimeZoneDbApiKeyUseCase, string>
    where TProvider : IApplicationTimeProvider
{
    private readonly IApplicationTimeRepository<TProvider> _repository = repository;

    public async Task<Result<string>> Handle(
        DropTimeZoneDbApiKeyUseCase useCase,
        CancellationToken ct = default
    )
    {
        string id = useCase.Message;
        Result removing = await _repository.Remove(id, ct);

        return removing.IsFailure
            ? Result.Failure<string>($"Не удается удалить ключ time zone db.")
            : id;
    }
}
