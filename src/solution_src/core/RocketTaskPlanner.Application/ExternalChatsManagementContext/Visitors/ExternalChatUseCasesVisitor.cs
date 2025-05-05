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

public sealed class ExternalChatUseCasesVisitor : IExternalChatUseCasesVisitor
{
    private readonly IUseCaseHandler<AddExternalChatOwnerUseCase, ExternalChatOwner> _addOwner;
    private readonly IUseCaseHandler<RemoveExternalChatUseCase, ExternalChat> _removeChat;
    private readonly IUseCaseHandler<AddExternalChatUseCase, ExternalChat> _addChat;
    private readonly IUseCaseHandler<AddExternalChatThemeUseCase, ExternalChat> _addTheme;
    private readonly IUseCaseHandler<RemoveExternalChatThemeUseCase, ExternalChat> _removeTheme;
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

    public async Task<Result> Visit(
        AddExternalChatUseCase useCase,
        CancellationToken ct = default
    ) => await _addChat.Handle(useCase, ct);

    public async Task<Result> Visit(
        AddExternalChatOwnerUseCase useCase,
        CancellationToken ct = default
    ) => await _addOwner.Handle(useCase, ct);

    public async Task<Result> Visit(
        RemoveExternalChatUseCase useCase,
        CancellationToken ct = default
    ) => await _removeChat.Handle(useCase, ct);

    public async Task<Result> Visit(
        RemoveExternalChatOwnerUseCase useCase,
        CancellationToken ct = default
    ) => await _removeOwner.Handle(useCase, ct);

    public async Task<Result> Visit(
        AddExternalChatThemeUseCase useCase,
        CancellationToken ct = default
    ) => await _addTheme.Handle(useCase, ct);

    public async Task<Result> Visit(
        RemoveExternalChatThemeUseCase useCase,
        CancellationToken ct = default
    ) => await _removeTheme.Handle(useCase, ct);
}
