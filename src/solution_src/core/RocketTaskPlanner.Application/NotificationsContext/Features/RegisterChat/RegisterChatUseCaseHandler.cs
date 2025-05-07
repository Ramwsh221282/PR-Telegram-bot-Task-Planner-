using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;
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

    /// <summary>
    /// <inheritdoc cref="IUnitOfWork"/>
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;

    public RegisterChatUseCaseHandler(
        INotificationsWritableRepository writableRepository,
        IUnitOfWork unitOfWork
    )
    {
        _writableRepository = writableRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RegisterChatUseCaseResponse>> Handle(
        RegisterChatUseCase useCase,
        CancellationToken ct = default
    )
    {
        NotificationReceiverId id = NotificationReceiverId.Create(useCase.ChatId).Value;
        NotificationReceiverName name = NotificationReceiverName.Create(useCase.ChatName).Value;
        NotificationReceiverTimeZone time = NotificationReceiverTimeZone
            .Create(useCase.ZoneName)
            .Value;

        NotificationReceiver receiver = new()
        {
            Id = id,
            Name = name,
            TimeZone = time,
        };

        Result saving = await _writableRepository.Add(receiver, _unitOfWork, ct);

        return saving.IsFailure
            ? Result.Failure<RegisterChatUseCaseResponse>(saving.Error)
            : new RegisterChatUseCaseResponse(id.Id, name.Name);
    }
}
