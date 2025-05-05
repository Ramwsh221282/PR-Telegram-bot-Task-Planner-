using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes.ValueObjects;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RemoveTheme;

public sealed class RemoveThemeUseCaseHandler : IUseCaseHandler<RemoveThemeUseCase, ReceiverTheme>
{
    private readonly INotificationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveThemeUseCaseHandler(INotificationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ReceiverTheme>> Handle(
        RemoveThemeUseCase useCase,
        CancellationToken ct = default
    )
    {
        var receiver = await _repository.Readable.GetById(useCase.ChatId, ct);
        if (receiver.IsFailure)
            return Result.Failure<ReceiverTheme>(receiver.Error);

        var id = ReceiverThemeId.Create(useCase.ThemeId).Value;
        var removed = receiver.Value.RemoveTheme(id);
        if (removed.IsFailure)
            return Result.Failure<ReceiverTheme>(removed.Error);

        var removing = _repository.Writable.RemoveTheme(removed.Value, _unitOfWork, ct);

        return removing.IsFailure
            ? Result.Failure<ReceiverTheme>("Не удалось удалить тему")
            : removed.Value;
    }
}
