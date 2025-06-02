using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
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
    private readonly IExternalChatsWritableRepository _repository;

    public RemoveExternalChatOwnerUseCaseHandler(
        IExternalChatsWritableRepository repository
    ) => _repository = repository;

    public async Task<Result<ExternalChatOwner>> Handle(
        RemoveExternalChatOwnerUseCase useCase,
        CancellationToken ct = default
    )
    {
        var owner = await _repository.GetById(useCase.OwnerId, ct);
        if (owner.IsFailure) return Result.Failure<ExternalChatOwner>(owner.Error);

        _repository.RemoveChatOwner(owner.Value);
        return owner;
    }
}
