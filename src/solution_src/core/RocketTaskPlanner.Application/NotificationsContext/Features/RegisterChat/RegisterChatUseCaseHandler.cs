using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RegisterChat;

/// <summary>
/// Создание основного чата
/// </summary>
/// <param name="writableRepository">Контракт взаимодействия с БД (операции записи)</param>
public sealed class RegisterChatUseCaseHandler(
    INotificationReceiverWritableRepository writableRepository
) : IUseCaseHandler<RegisterChatUseCase, RegisterChatUseCaseResponse>
{
    private readonly INotificationReceiverWritableRepository _writableRepository =
        writableRepository;

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

        Result saving = await _writableRepository.Add(receiver, ct);

        return saving.IsFailure
            ? Result.Failure<RegisterChatUseCaseResponse>(saving.Error)
            : new RegisterChatUseCaseResponse(id.Id, name.Name);
    }
}
