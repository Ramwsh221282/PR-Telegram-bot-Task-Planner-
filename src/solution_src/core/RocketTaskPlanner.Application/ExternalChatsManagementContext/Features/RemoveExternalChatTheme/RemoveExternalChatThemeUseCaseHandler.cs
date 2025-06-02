using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.Entities;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChatTheme;

/// <summary>
/// Обработчик для <inheritdoc cref="RemoveExternalChatThemeUseCase"/>
/// </summary>
public sealed class RemoveExternalChatThemeUseCaseHandler
    : IUseCaseHandler<RemoveExternalChatThemeUseCase, ExternalChat>
{
    /// <summary>
    /// <inheritdoc cref="IExternalChatsReadableRepository"/>
    /// </summary>
    private readonly IExternalChatsWritableRepository _repository;

    public RemoveExternalChatThemeUseCaseHandler(IExternalChatsWritableRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ExternalChat>> Handle(
        RemoveExternalChatThemeUseCase useCase,
        CancellationToken ct = default
    )
    {
        var owner = await _repository.GetById(useCase.userId, ct);
        if (owner.IsFailure) return Result.Failure<ExternalChat>(owner.Error);

        var themeChat = owner.Value.GetChildChat(
            ExternalChatId.Dedicated(useCase.chatId),
            ExternalChatId.Dedicated(useCase.themeId));
        
        if (themeChat.IsFailure) return Result.Failure<ExternalChat>(themeChat.Error);

        var removing = owner.Value.RemoveExternalChat(themeChat.Value.Id);
        return removing.IsFailure ? Result.Failure<ExternalChat>(removing.Error) : themeChat;
    }
}
