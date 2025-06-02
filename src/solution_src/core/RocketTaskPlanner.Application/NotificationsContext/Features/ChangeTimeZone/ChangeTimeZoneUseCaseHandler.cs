using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.ChangeTimeZone;

/// <summary>
/// Обработчик <inheritdoc cref="ChangeTimeZoneUseCase"/>
/// </summary>
public sealed class ChangeTimeZoneUseCaseHandler
    : IUseCaseHandler<ChangeTimeZoneUseCase, NotificationReceiverTimeZone>
{
    /// <summary>
    /// <inheritdoc cref="INotificationsWritableRepository"/>
    /// </summary>
    private readonly INotificationsWritableRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeTimeZoneUseCaseHandler(INotificationsWritableRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<NotificationReceiverTimeZone>> Handle(
        ChangeTimeZoneUseCase useCase,
        CancellationToken ct = default
    )
    {
        var receiver = await _repository.GetById(useCase.ChatId, ct);
        if (receiver.IsFailure) return Result.Failure<NotificationReceiverTimeZone>(receiver.Error);

        await _unitOfWork.BeginTransaction(ct);
        var timeZone = NotificationReceiverTimeZone.Create(useCase.ZoneName).Value;
        receiver.Value.TimeZone = timeZone;
        
        var savingChanges = await _unitOfWork.SaveChangesAsync(ct);
        if (savingChanges.IsFailure)
        {
            await _unitOfWork.RollBackTransaction(ct);
            return Result.Failure<NotificationReceiverTimeZone>(savingChanges.Error);
        }
        
        var committing = await _unitOfWork.CommitTransaction(ct);
        return committing.IsFailure ? Result.Failure<NotificationReceiverTimeZone>(committing.Error) : timeZone; 
    }
}
