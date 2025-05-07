using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;
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
    /// <summary>
    /// <inheritdoc cref="IExternalChatsReadableRepository"/>
    /// </summary>
    private readonly IExternalChatsRepository _repository;

    /// <summary>
    /// <inheritdoc cref="IUnitOfWork"/>
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;

    public AddExternalChatUseCaseHandler(
        IExternalChatsRepository repository,
        IUnitOfWork unitOfWork
    )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ExternalChat>> Handle(
        AddExternalChatUseCase useCase,
        CancellationToken ct = default
    )
    {
        ExternalChatMemberId ownerId = ExternalChatMemberId.Dedicated(useCase.OwnerId);
        ExternalChatId chatId = ExternalChatId.Dedicated(useCase.ParentChatId);
        ExternalChatName chatName = ExternalChatName.Create(useCase.ChatName).Value;
        ExternalChat chat = new(ownerId, chatId, chatName);

        _repository.Writable.AddChat(chat, _unitOfWork, ct);
        return await Task.FromResult(chat);
    }
}
