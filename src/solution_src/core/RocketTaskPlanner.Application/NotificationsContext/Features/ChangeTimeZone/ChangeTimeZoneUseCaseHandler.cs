using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.ChangeTimeZone;

/// <summary>
/// Обработчик изменения временной зоны чата.
/// </summary>
public sealed class ChangeTimeZoneUseCaseHandler
    : IUseCaseHandler<ChangeTimeZoneUseCase, NotificationReceiverTimeZone>
{
    private readonly INotificationsWritableRepository _repository;

    public ChangeTimeZoneUseCaseHandler(INotificationsWritableRepository repository) =>
        _repository = repository;

    public async Task<Result<NotificationReceiverTimeZone>> Handle(
        ChangeTimeZoneUseCase useCase,
        CancellationToken ct = default
    )
    {
        var timeZone = NotificationReceiverTimeZone.Create(useCase.ZoneName).Value;
        var id = NotificationReceiverId.Create(useCase.ChatId).Value;

        var chaningTimeZone = await _repository.ChangeTimeZone(id, timeZone, ct);

        return chaningTimeZone.IsSuccess
            ? timeZone
            : Result.Failure<NotificationReceiverTimeZone>(chaningTimeZone.Error);
    }
}
