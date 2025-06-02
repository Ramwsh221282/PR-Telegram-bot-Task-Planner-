using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.Entities;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChat;

/// <summary>
/// Обработчик для <inheritdoc cref="RemoveExternalChatUseCase"/>
/// </summary>
public sealed class RemoveExternalChatUseCaseHandler
    : IUseCaseHandler<RemoveExternalChatUseCase, ExternalChat>
{
    /// <summary>
    /// <inheritdoc cref="IExternalChatsReadableRepository"/>
    /// </summary>
    private readonly IExternalChatsWritableRepository _repository;

    public RemoveExternalChatUseCaseHandler(IExternalChatsWritableRepository repository) =>
        _repository = repository;

    public async Task<Result<ExternalChat>> Handle(
        RemoveExternalChatUseCase useCase,
        CancellationToken ct = default
    )
    {
        var owner = await _repository.GetById(useCase.ownerId, ct);
        if (owner.IsFailure) return Result.Failure<ExternalChat>(owner.Error);

        ExternalChatId chatId = ExternalChatId.Dedicated(useCase.chatId);
        var removed = owner.Value.RemoveExternalChat(chatId);
        return removed.IsFailure ? Result.Failure<ExternalChat>(removed.Error) : removed.Value;
    }
}
