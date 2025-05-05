using RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChat;
using RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChatTheme;
using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterChat;
using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterTheme;
using RocketTaskPlanner.Application.NotificationsContext.Features.RemoveChat;
using RocketTaskPlanner.Application.NotificationsContext.Features.RemoveTheme;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;

namespace RocketTaskPlanner.Application.NotificationsContext.Visitor;

public sealed class NotificationUseCaseVisitor : INotificationUseCaseVisitor
{
    private readonly IUseCaseHandler<
        CreateTaskForChatUseCase,
        CreateTaskForChatUseCaseResponse
    > _chatTask;

    private readonly IUseCaseHandler<
        CreateTaskForChatThemeUseCase,
        CreateTaskForChatThemeUseCaseResponse
    > _themeTask;

    private readonly IUseCaseHandler<RegisterChatUseCase, RegisterChatUseCaseResponse> _addChat;

    private readonly IUseCaseHandler<RegisterThemeUseCase, RegisterThemeResponse> _registerTheme;

    private readonly IUseCaseHandler<RemoveThemeUseCase, ReceiverTheme> _removeTheme;

    private readonly IUseCaseHandler<RemoveChatUseCase, NotificationReceiver> _removeChat;

    public NotificationUseCaseVisitor(
        IUseCaseHandler<CreateTaskForChatUseCase, CreateTaskForChatUseCaseResponse> chatTask,
        IUseCaseHandler<
            CreateTaskForChatThemeUseCase,
            CreateTaskForChatThemeUseCaseResponse
        > themeTask,
        IUseCaseHandler<RegisterChatUseCase, RegisterChatUseCaseResponse> addChat,
        IUseCaseHandler<RegisterThemeUseCase, RegisterThemeResponse> registerTheme,
        IUseCaseHandler<RemoveThemeUseCase, ReceiverTheme> removeTheme,
        IUseCaseHandler<RemoveChatUseCase, NotificationReceiver> removeChat
    )
    {
        _chatTask = chatTask;
        _themeTask = themeTask;
        _addChat = addChat;
        _registerTheme = registerTheme;
        _removeTheme = removeTheme;
        _removeChat = removeChat;
    }

    public async Task<Result> Visit(
        CreateTaskForChatUseCase useCase,
        CancellationToken ct = default
    ) => await _chatTask.Handle(useCase, ct);

    public async Task<Result> Visit(
        CreateTaskForChatThemeUseCase useCase,
        CancellationToken ct = default
    ) => await _themeTask.Handle(useCase, ct);

    public async Task<Result> Visit(RegisterChatUseCase useCase, CancellationToken ct = default) =>
        await _addChat.Handle(useCase, ct);

    public async Task<Result> Visit(RegisterThemeUseCase useCase, CancellationToken ct = default) =>
        await _registerTheme.Handle(useCase, ct);

    public async Task<Result> Visit(RemoveThemeUseCase useCase, CancellationToken ct = default) =>
        await _removeTheme.Handle(useCase, ct);

    public async Task<Result> Visit(RemoveChatUseCase useCase, CancellationToken ct = default) =>
        await _removeChat.Handle(useCase, ct);
}
