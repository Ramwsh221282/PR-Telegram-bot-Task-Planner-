using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes.ValueObjects;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RegisterTheme;

/// <summary>
/// Обработчик для <inheritdoc cref="RegisterThemeUseCase"/>
/// </summary>
public sealed class RegisterThemeUseCaseHandler
    : IUseCaseHandler<RegisterThemeUseCase, RegisterThemeResponse>
{
    /// <summary>
    /// <inheritdoc cref="INotificationsReadableRepository"/>
    /// </summary>
    private readonly INotificationsWritableRepository _repository;

    public RegisterThemeUseCaseHandler(INotificationsWritableRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<RegisterThemeResponse>> Handle(
        RegisterThemeUseCase useCase,
        CancellationToken ct = default
    )
    {
        var receiver = await _repository.GetById(useCase.ChatId, ct);
        if (receiver.IsFailure) return Result.Failure<RegisterThemeResponse>(receiver.Error);

        var id = ReceiverThemeId.Create(useCase.ThemeId).Value;
        var theme = receiver.Value.AddTheme(id);
        
        return theme.IsFailure ?
            Result.Failure<RegisterThemeResponse>(theme.Error) :
            new RegisterThemeResponse(useCase.ChatId, receiver.Value.Name.Name, id.Id);
    }
}
