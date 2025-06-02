using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes.ValueObjects;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RemoveTheme;

/// <summary>
/// Обработчик для <inheritdoc cref="RemoveThemeUseCase"/>
/// </summary>
public sealed class RemoveThemeUseCaseHandler : IUseCaseHandler<RemoveThemeUseCase, ReceiverTheme>
{
    private readonly INotificationsWritableRepository _repository;

    public RemoveThemeUseCaseHandler(INotificationsWritableRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ReceiverTheme>> Handle(
        RemoveThemeUseCase useCase,
        CancellationToken ct = default
    )
    {
        var receiver = await _repository.GetById(useCase.ChatId, ct);
        if (receiver.IsFailure) return Result.Failure<ReceiverTheme>(receiver.Error);

        var id = ReceiverThemeId.Create(useCase.ThemeId).Value;
        var removed = receiver.Value.RemoveTheme(id);
        return removed.IsFailure ? Result.Failure<ReceiverTheme>(removed.Error) : removed.Value;
    }
}
