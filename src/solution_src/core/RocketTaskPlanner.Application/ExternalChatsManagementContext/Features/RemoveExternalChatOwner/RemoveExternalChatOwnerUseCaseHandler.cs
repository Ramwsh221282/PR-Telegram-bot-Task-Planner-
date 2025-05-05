using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChatOwner;

public sealed class RemoveExternalChatOwnerUseCaseHandler
    : IUseCaseHandler<RemoveExternalChatOwnerUseCase, ExternalChatOwner>
{
    private readonly IExternalChatsRepository _repository;
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
