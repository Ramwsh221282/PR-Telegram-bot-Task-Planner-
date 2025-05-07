using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;
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
    private readonly INotificationRepository _repository;

    /// <summary>
    /// <inheritdoc cref="IUnitOfWork"/>
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;

    public RegisterThemeUseCaseHandler(INotificationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RegisterThemeResponse>> Handle(
        RegisterThemeUseCase useCase,
        CancellationToken ct = default
    )
    {
        var receiver = await _repository.Readable.GetById(useCase.ChatId, ct);
        if (receiver.IsFailure)
            return Result.Failure<RegisterThemeResponse>(receiver.Error);

        var id = ReceiverThemeId.Create(useCase.ThemeId).Value;
        var theme = receiver.Value.AddTheme(id);
        if (theme.IsFailure)
            return Result.Failure<RegisterThemeResponse>(theme.Error);

        var result = _repository.Writable.AddTheme(theme.Value, _unitOfWork, ct);

        return result.IsFailure
            ? Result.Failure<RegisterThemeResponse>(result.Error)
            : new RegisterThemeResponse(useCase.ChatId, receiver.Value.Name.Name, id.Id);
    }
}
