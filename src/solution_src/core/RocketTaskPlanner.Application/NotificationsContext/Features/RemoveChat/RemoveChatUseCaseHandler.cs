using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.NotificationsContext;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RemoveChat;

/// <summary>
/// Обработчик для <inheritdoc cref="RemoveChatUseCase"/>
/// </summary>
public sealed class RemoveChatUseCaseHandler
    : IUseCaseHandler<RemoveChatUseCase, NotificationReceiver>
{
    private readonly INotificationsWritableRepository _repository;

    public RemoveChatUseCaseHandler(INotificationsWritableRepository repository) =>
        _repository = repository;

    public async Task<Result<NotificationReceiver>> Handle(
        RemoveChatUseCase useCase,
        CancellationToken ct = default
    )
    {
        var receiver = await _repository.GetById(useCase.chatId, ct);
        if (receiver.IsFailure) return Result.Failure<NotificationReceiver>(receiver.Error);

        _repository.Remove(receiver.Value);
        return receiver.Value;
    }
}
