using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RegisterChat;

/// <summary>
/// Обработчик для <inheritdoc cref="RegisterChatUseCase"/>
/// </summary>
public sealed class RegisterChatUseCaseHandler
    : IUseCaseHandler<RegisterChatUseCase, RegisterChatUseCaseResponse>
{
    /// <summary>
    /// <inheritdoc cref="INotificationsWritableRepository"/>
    /// </summary>
    private readonly INotificationsWritableRepository _writableRepository;
    

    public RegisterChatUseCaseHandler(INotificationsWritableRepository writableRepository)
    {
        _writableRepository = writableRepository;
    }

    public async Task<Result<RegisterChatUseCaseResponse>> Handle(
        RegisterChatUseCase useCase,
        CancellationToken ct = default
    )
    {
        var id = NotificationReceiverId.Create(useCase.ChatId).Value;
        var name = NotificationReceiverName.Create(useCase.ChatName).Value;
        var time = NotificationReceiverTimeZone.Create(useCase.ZoneName).Value;

        NotificationReceiver receiver = new()
        {
            Id = id,
            Name = name,
            TimeZone = time,
        };

        var saving = await _writableRepository.Add(receiver, ct);
        return saving.IsFailure
            ? Result.Failure<RegisterChatUseCaseResponse>(saving.Error)
            : new RegisterChatUseCaseResponse(id.Id, name.Name);
    }
}
