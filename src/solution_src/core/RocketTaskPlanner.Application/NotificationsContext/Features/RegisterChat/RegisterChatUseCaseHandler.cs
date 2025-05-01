using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RegisterChat;

public sealed class RegisterChatUseCaseHandler(INotificationReceiverRepository repository)
    : IUseCaseHandler<RegisterChatUseCase, RegisterChatUseCaseResponse>
{
    private readonly INotificationReceiverRepository _repository = repository;

    public async Task<Result<RegisterChatUseCaseResponse>> Handle(
        RegisterChatUseCase useCase,
        CancellationToken ct = default
    )
    {
        NotificationReceiverId id = NotificationReceiverId.Create(useCase.ChatId).Value;
        NotificationReceiverName name = NotificationReceiverName.Create(useCase.ChatName).Value;
        NotificationReceiverTimeZone time = NotificationReceiverTimeZone
            .Create(useCase.ZoneName, useCase.TimeStamp)
            .Value;

        NotificationReceiver receiver = new()
        {
            Id = id,
            Name = name,
            TimeZone = time,
        };

        Result saving = await _repository.Add(receiver, ct);
        if (saving.IsFailure)
            return Result.Failure<RegisterChatUseCaseResponse>(saving.Error);

        return new RegisterChatUseCaseResponse(id.Id, name.Name);
    }
}
