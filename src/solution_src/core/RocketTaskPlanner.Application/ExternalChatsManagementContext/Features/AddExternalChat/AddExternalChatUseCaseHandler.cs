using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.Entities;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChat;

/// <summary>
/// Обработчик <inheritdoc cref="AddExternalChatUseCase"/>
/// </summary>
public sealed class AddExternalChatUseCaseHandler
    : IUseCaseHandler<AddExternalChatUseCase, ExternalChat>
{
    private readonly IExternalChatsWritableRepository _repository;

    public AddExternalChatUseCaseHandler(IExternalChatsWritableRepository repository) =>
        _repository = repository;

    public async Task<Result<ExternalChat>> Handle(
        AddExternalChatUseCase useCase,
        CancellationToken ct = default
    )
    {
        ExternalChatMemberId ownerId = ExternalChatMemberId.Dedicated(useCase.OwnerId);
        var owner = await _repository.GetById(ownerId.Value, ct);
        if (owner.IsFailure) return Result.Failure<ExternalChat>(owner.Error);
        
        ExternalChatId chatId = ExternalChatId.Dedicated(useCase.ParentChatId);
        ExternalChatName chatName = ExternalChatName.Create(useCase.ChatName).Value;
        var chatResult = owner.Value.AddExternalChat(chatId, chatName);
        
        return chatResult.IsFailure ? Result.Failure<ExternalChat>(chatResult.Error) : chatResult.Value;
    }
}
