using RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChat;
using RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChatTheme;
using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterChat;
using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterTheme;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;

namespace RocketTaskPlanner.Application.NotificationsContext.Visitor;

public sealed class NotificationUseCaseVisitor : INotificationUseCaseVisitor
{
    private readonly IUseCaseHandler<
        CreateTaskForChatUseCase,
        CreateTaskForChatUseCaseResponse
    > _createTaskForGeneralChatUseCase;

    private readonly IUseCaseHandler<
        CreateTaskForChatThemeUseCase,
        CreateTaskForChatThemeUseCaseResponse
    > _createTaskForChatThemeUseCase;

    private readonly IUseCaseHandler<
        RegisterChatUseCase,
        RegisterChatUseCaseResponse
    > _registerChatUseCase;

    private readonly IUseCaseHandler<
        RegisterThemeUseCase,
        RegisterThemeResponse
    > _registerThemeUseCase;

    public NotificationUseCaseVisitor(
        IUseCaseHandler<
            CreateTaskForChatUseCase,
            CreateTaskForChatUseCaseResponse
        > createTaskForGeneralChatUseCase,
        IUseCaseHandler<
            CreateTaskForChatThemeUseCase,
            CreateTaskForChatThemeUseCaseResponse
        > createTaskForChatThemeUseCase,
        IUseCaseHandler<RegisterChatUseCase, RegisterChatUseCaseResponse> registerChatUseCase,
        IUseCaseHandler<RegisterThemeUseCase, RegisterThemeResponse> registerThemeUseCase
    )
    {
        _createTaskForGeneralChatUseCase = createTaskForGeneralChatUseCase;
        _createTaskForChatThemeUseCase = createTaskForChatThemeUseCase;
        _registerChatUseCase = registerChatUseCase;
        _registerThemeUseCase = registerThemeUseCase;
    }

    public async Task<Result> Visit(
        CreateTaskForChatUseCase useCase,
        CancellationToken ct = default
    ) => await _createTaskForGeneralChatUseCase.Handle(useCase, ct);

    public async Task<Result> Visit(
        CreateTaskForChatThemeUseCase useCase,
        CancellationToken ct = default
    ) => await _createTaskForChatThemeUseCase.Handle(useCase, ct);

    public async Task<Result> Visit(RegisterChatUseCase useCase, CancellationToken ct = default) =>
        await _registerChatUseCase.Handle(useCase, ct);

    public async Task<Result> Visit(RegisterThemeUseCase useCase, CancellationToken ct = default) =>
        await _registerThemeUseCase.Handle(useCase, ct);
}
