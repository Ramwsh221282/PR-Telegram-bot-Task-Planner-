using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.Entities;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChat;

/// <summary>
/// Обработчик удаления внешнего чата у обладателя
/// </summary>
public sealed class RemoveExternalChatUseCaseHandler
    : IUseCaseHandler<RemoveExternalChatUseCase, ExternalChat>
{
    private readonly IExternalChatsRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveExternalChatUseCaseHandler(
        IExternalChatsRepository repository,
        IUnitOfWork unitOfWork
    )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ExternalChat>> Handle(
        RemoveExternalChatUseCase useCase,
        CancellationToken ct = default
    )
    {
        Result<ExternalChatOwner> ownerResult = await _repository.Readable.GetExternalChatOwnerById(
            useCase.ownerId,
            ct
        );
        if (ownerResult.IsFailure)
            return Result.Failure<ExternalChat>(ownerResult.Error);

        ExternalChatId chatId = ExternalChatId.Dedicated(useCase.chatId);
        Result<ExternalChat> removed = ownerResult.Value.RemoveExternalChat(chatId);
        if (removed.IsFailure)
            return Result.Failure<ExternalChat>(removed.Error);

        _repository.Writable.RemoveChat(removed.Value, _unitOfWork, ct);
        return removed;
    }
}
