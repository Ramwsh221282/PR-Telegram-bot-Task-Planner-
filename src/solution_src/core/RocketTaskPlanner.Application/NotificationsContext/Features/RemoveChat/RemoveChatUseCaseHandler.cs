using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.NotificationsContext;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RemoveChat;

/// <summary>
/// Обработчик для <inheritdoc cref="RemoveChatUseCase"/>
/// </summary>
public sealed class RemoveChatUseCaseHandler
    : IUseCaseHandler<RemoveChatUseCase, NotificationReceiver>
{
    /// <summary>
    /// <inheritdoc cref="INotificationRepository"/>
    /// </summary>
    private readonly INotificationRepository _repository;

    /// <summary>
    /// <inheritdoc cref="IUnitOfWork"/>
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;

    public RemoveChatUseCaseHandler(INotificationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<NotificationReceiver>> Handle(
        RemoveChatUseCase useCase,
        CancellationToken ct = default
    )
    {
        var receiver = await _repository.Readable.GetById(useCase.chatId, ct);
        if (receiver.IsFailure)
            return Result.Failure<NotificationReceiver>(receiver.Error);

        var result = _repository.Writable.Remove(receiver.Value.Id.Id, _unitOfWork, ct);

        return result.IsFailure
            ? Result.Failure<NotificationReceiver>(result.Error)
            : receiver.Value;
    }
}
