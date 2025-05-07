using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RemoveThemeSubject;

/// <summary>
/// Обработчик для <inheritdoc cref="RemoveThemeSubjectUseCase"/>
/// </summary>
public sealed class RemoveThemeSubjectUseCaseHandler
    : IUseCaseHandler<RemoveThemeSubjectUseCase, bool>
{
    /// <summary>
    /// <inheritdoc cref="INotificationsWritableRepository"/>
    /// </summary>
    private readonly INotificationsWritableRepository _repository;

    public RemoveThemeSubjectUseCaseHandler(INotificationsWritableRepository repository) =>
        _repository = repository;

    public async Task<Result<bool>> Handle(
        RemoveThemeSubjectUseCase useCase,
        CancellationToken ct = default
    )
    {
        long subjectId = useCase.SubjectId;
        Result deleting = await _repository.RemoveThemeChatSubject(subjectId, ct);
        return deleting.IsFailure ? Result.Failure<bool>(deleting.Error) : true;
    }
}
