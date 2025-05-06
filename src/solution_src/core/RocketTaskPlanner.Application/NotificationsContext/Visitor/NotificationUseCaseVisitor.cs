using RocketTaskPlanner.Application.NotificationsContext.Features.ChangeTimeZone;
using RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChat;
using RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChatTheme;
using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterChat;
using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterTheme;
using RocketTaskPlanner.Application.NotificationsContext.Features.RemoveChat;
using RocketTaskPlanner.Application.NotificationsContext.Features.RemoveChatSubject;
using RocketTaskPlanner.Application.NotificationsContext.Features.RemoveTheme;
using RocketTaskPlanner.Application.NotificationsContext.Features.RemoveThemeSubject;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

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

    private readonly IUseCaseHandler<
        ChangeTimeZoneUseCase,
        NotificationReceiverTimeZone
    > _changeTimeZone;

    private readonly IUseCaseHandler<RegisterChatUseCase, RegisterChatUseCaseResponse> _addChat;

    private readonly IUseCaseHandler<RegisterThemeUseCase, RegisterThemeResponse> _registerTheme;

    private readonly IUseCaseHandler<RemoveThemeUseCase, ReceiverTheme> _removeTheme;

    private readonly IUseCaseHandler<RemoveChatUseCase, NotificationReceiver> _removeChat;

    private readonly IUseCaseHandler<RemoveChatSubjectUseCase, bool> _removeChatSubject;

    private readonly IUseCaseHandler<RemoveThemeSubjectUseCase, bool> _removeThemeSubject;

    public NotificationUseCaseVisitor(
        IUseCaseHandler<
            CreateTaskForChatThemeUseCase,
            CreateTaskForChatThemeUseCaseResponse
        > themeTask,
        IUseCaseHandler<CreateTaskForChatUseCase, CreateTaskForChatUseCaseResponse> chatTask,
        IUseCaseHandler<ChangeTimeZoneUseCase, NotificationReceiverTimeZone> changeTimeZone,
        IUseCaseHandler<RegisterChatUseCase, RegisterChatUseCaseResponse> addChat,
        IUseCaseHandler<RegisterThemeUseCase, RegisterThemeResponse> registerTheme,
        IUseCaseHandler<RemoveThemeUseCase, ReceiverTheme> removeTheme,
        IUseCaseHandler<RemoveChatUseCase, NotificationReceiver> removeChat,
        IUseCaseHandler<RemoveChatSubjectUseCase, bool> removeChatSubject,
        IUseCaseHandler<RemoveThemeSubjectUseCase, bool> removeThemeSubject
    )
    {
        _chatTask = chatTask;
        _themeTask = themeTask;
        _addChat = addChat;
        _registerTheme = registerTheme;
        _removeTheme = removeTheme;
        _removeChat = removeChat;
        _changeTimeZone = changeTimeZone;
        _removeChatSubject = removeChatSubject;
        _removeThemeSubject = removeThemeSubject;
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

    public async Task<Result> Visit(
        ChangeTimeZoneUseCase useCase,
        CancellationToken ct = default
    ) => await _changeTimeZone.Handle(useCase, ct);

    public async Task<Result> Visit(
        RemoveChatSubjectUseCase useCase,
        CancellationToken ct = default
    ) => await _removeChatSubject.Handle(useCase, ct);

    public async Task<Result> Visit(
        RemoveThemeSubjectUseCase useCase,
        CancellationToken ct = default
    ) => await _removeThemeSubject.Handle(useCase, ct);
}
