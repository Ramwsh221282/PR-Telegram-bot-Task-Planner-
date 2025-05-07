using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.Entities;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChatTheme;

/// <summary>
/// Обработчик для <inheritdoc cref="AddExternalChatThemeUseCase"/>
/// </summary>
public sealed class AddExternalChatThemeUseCaseHandler
    : IUseCaseHandler<AddExternalChatThemeUseCase, ExternalChat>
{
    /// <summary>
    /// <inheritdoc cref="IExternalChatsReadableRepository"/>
    /// </summary>
    private readonly IExternalChatsRepository _repository;

    /// <summary>
    /// <inheritdoc cref="IUnitOfWork"/>
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;

    public AddExternalChatThemeUseCaseHandler(
        IExternalChatsRepository repository,
        IUnitOfWork unitOfWork
    )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ExternalChat>> Handle(
        AddExternalChatThemeUseCase useCase,
        CancellationToken ct = default
    )
    {
        var owner = await _repository.Readable.GetExternalChatOwnerById(useCase.OwnerId, ct);
        if (owner.IsFailure)
            return Result.Failure<ExternalChat>(owner.Error);

        var parentId = ExternalChatId.Dedicated(useCase.ParentChatId);
        var themeId = ExternalChatId.Dedicated(useCase.ThemeId);
        var name = ExternalChatName.Create($"Тема чата: {useCase.ChatName}").Value;

        var parentChat = owner.Value.GetParentChat(parentId);
        if (parentChat.IsFailure)
            return Result.Failure<ExternalChat>(owner.Error);

        var createdThemeChat = owner.Value.AddExternalThemeChat(themeId, name, parentChat.Value);
        if (createdThemeChat.IsFailure)
            return Result.Failure<ExternalChat>(createdThemeChat.Error);

        _repository.Writable.AddThemeChat(createdThemeChat.Value, _unitOfWork, ct);
        return createdThemeChat;
    }
}
