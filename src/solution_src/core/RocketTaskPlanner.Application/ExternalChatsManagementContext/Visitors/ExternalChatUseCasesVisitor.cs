using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChat;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChatOwner;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChatTheme;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChat;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChatOwner;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChatTheme;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.Entities;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;

/// <summary>
/// Посетитель бизнес-логических операций связанных с объектом - <inheritdoc cref="ExternalChatOwner"/>
/// </summary>
public sealed class ExternalChatUseCasesVisitor : IExternalChatUseCasesVisitor
{
    /// <summary>
    /// <inheritdoc cref="AddExternalChatOwnerUseCaseHandler"/>
    /// </summary>
    private readonly IUseCaseHandler<AddExternalChatOwnerUseCase, ExternalChatOwner> _addOwner;

    /// <summary>
    /// <inheritdoc cref="RemoveExternalChatUseCaseHandler"/>
    /// </summary>
    private readonly IUseCaseHandler<RemoveExternalChatUseCase, ExternalChat> _removeChat;

    /// <summary>
    /// <inheritdoc cref="AddExternalChatOwnerUseCaseHandler"/>
    /// </summary>
    private readonly IUseCaseHandler<AddExternalChatUseCase, ExternalChat> _addChat;

    /// <summary>
    /// <inheritdoc cref="AddExternalChatThemeUseCaseHandler"/>
    /// </summary>
    private readonly IUseCaseHandler<AddExternalChatThemeUseCase, ExternalChat> _addTheme;

    /// <summary>
    /// <inheritdoc cref="RemoveExternalChatThemeUseCaseHandler"/>
    /// </summary>
    private readonly IUseCaseHandler<RemoveExternalChatThemeUseCase, ExternalChat> _removeTheme;

    /// <summary>
    /// <inheritdoc cref="RemoveExternalChatOwnerUseCaseHandler"/>
    /// </summary>
    private readonly IUseCaseHandler<
        RemoveExternalChatOwnerUseCase,
        ExternalChatOwner
    > _removeOwner;

    public ExternalChatUseCasesVisitor(
        IUseCaseHandler<AddExternalChatOwnerUseCase, ExternalChatOwner> addOwner,
        IUseCaseHandler<RemoveExternalChatUseCase, ExternalChat> removeChat,
        IUseCaseHandler<AddExternalChatUseCase, ExternalChat> addChat,
        IUseCaseHandler<AddExternalChatThemeUseCase, ExternalChat> addTheme,
        IUseCaseHandler<RemoveExternalChatThemeUseCase, ExternalChat> removeTheme,
        IUseCaseHandler<RemoveExternalChatOwnerUseCase, ExternalChatOwner> removeOwner
    )
    {
        _addOwner = addOwner;
        _removeChat = removeChat;
        _addChat = addChat;
        _addTheme = addTheme;
        _removeTheme = removeTheme;
        _removeOwner = removeOwner;
    }

    /// <summary>
    /// <inheritdoc cref="IExternalChatUseCasesVisitor.Visit(RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChat.AddExternalChatUseCase,System.Threading.CancellationToken)"/>
    /// </summary>
    public async Task<Result> Visit(
        AddExternalChatUseCase useCase,
        CancellationToken ct = default
    ) => await _addChat.Handle(useCase, ct);

    /// <summary>
    /// <inheritdoc cref="IExternalChatUseCasesVisitor.Visit(RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChatOwner.AddExternalChatOwnerUseCase,System.Threading.CancellationToken)"/>
    /// </summary>
    public async Task<Result> Visit(
        AddExternalChatOwnerUseCase useCase,
        CancellationToken ct = default
    ) => await _addOwner.Handle(useCase, ct);

    /// <summary>
    /// <inheritdoc cref="IExternalChatUseCasesVisitor.Visit(RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChat.RemoveExternalChatUseCase,System.Threading.CancellationToken)"/>
    /// </summary>
    public async Task<Result> Visit(
        RemoveExternalChatUseCase useCase,
        CancellationToken ct = default
    ) => await _removeChat.Handle(useCase, ct);

    /// <summary>
    /// <inheritdoc cref="IExternalChatUseCasesVisitor.Visit(RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChatOwner.RemoveExternalChatOwnerUseCase,System.Threading.CancellationToken)"/>
    /// </summary>
    public async Task<Result> Visit(
        RemoveExternalChatOwnerUseCase useCase,
        CancellationToken ct = default
    ) => await _removeOwner.Handle(useCase, ct);

    /// <summary>
    /// <inheritdoc cref="IExternalChatUseCasesVisitor.Visit(RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChatTheme.AddExternalChatThemeUseCase,System.Threading.CancellationToken)"/>
    /// </summary>
    public async Task<Result> Visit(
        AddExternalChatThemeUseCase useCase,
        CancellationToken ct = default
    ) => await _addTheme.Handle(useCase, ct);

    /// <summary>
    /// <inheritdoc cref="IExternalChatUseCasesVisitor"/>
    /// </summary>
    public async Task<Result> Visit(
        RemoveExternalChatThemeUseCase useCase,
        CancellationToken ct = default
    ) => await _removeTheme.Handle(useCase, ct);
}
