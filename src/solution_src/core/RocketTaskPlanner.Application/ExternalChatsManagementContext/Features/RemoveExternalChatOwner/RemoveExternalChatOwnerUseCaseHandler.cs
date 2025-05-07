using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChatOwner;

/// <summary>
/// Обработчик для <inheritdoc cref="RemoveExternalChatOwnerUseCase"/>
/// </summary>
public sealed class RemoveExternalChatOwnerUseCaseHandler
    : IUseCaseHandler<RemoveExternalChatOwnerUseCase, ExternalChatOwner>
{
    /// <summary>
    /// <inheritdoc cref="IExternalChatsReadableRepository"/>
    /// </summary>
    private readonly IExternalChatsRepository _repository;

    /// <summary>
    /// <inheritdoc cref="IUnitOfWork"/>
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;

    public RemoveExternalChatOwnerUseCaseHandler(
        IExternalChatsRepository repository,
        IUnitOfWork unitOfWork
    )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ExternalChatOwner>> Handle(
        RemoveExternalChatOwnerUseCase useCase,
        CancellationToken ct = default
    )
    {
        Result<ExternalChatOwner> owner = await _repository.Readable.GetExternalChatOwnerById(
            useCase.OwnerId,
            ct
        );
        if (owner.IsFailure)
            return Result.Failure<ExternalChatOwner>(owner.Error);

        _repository.Writable.RemoveChatOwner(owner.Value, _unitOfWork, ct);
        return owner;
    }
}
