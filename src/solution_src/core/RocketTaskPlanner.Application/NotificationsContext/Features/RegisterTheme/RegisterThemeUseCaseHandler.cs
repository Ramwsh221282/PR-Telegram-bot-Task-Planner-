using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes.ValueObjects;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RegisterTheme;

public sealed class RegisterThemeUseCaseHandler(INotificationReceiverRepository repository)
    : IUseCaseHandler<RegisterThemeUseCase, RegisterThemeResponse>
{
    private readonly INotificationReceiverRepository _repository = repository;

    public async Task<Result<RegisterThemeResponse>> Handle(
        RegisterThemeUseCase useCase,
        CancellationToken ct = default
    )
    {
        Result<NotificationReceiver> receiver = await _repository.GetById(useCase.ChatId, ct);
        if (receiver.IsFailure)
            return Result.Failure<RegisterThemeResponse>(receiver.Error);

        ReceiverThemeId id = ReceiverThemeId.Create(useCase.ThemeId).Value;
        Result<ReceiverTheme> theme = receiver.Value.AddTheme(id);
        if (theme.IsFailure)
            return Result.Failure<RegisterThemeResponse>(theme.Error);

        await _repository.AddTheme(theme.Value, ct);
        return new RegisterThemeResponse(useCase.ChatId, receiver.Value.Name.Name, id.Id);
    }
}
