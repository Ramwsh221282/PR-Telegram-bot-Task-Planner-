using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RemoveChatSubject;

/// <summary>
/// Обработчик для <inheritdoc cref="RemoveChatSubjectUseCase"/>
/// </summary>
public sealed class RemoveChatSubjectUseCaseHandler
    : IUseCaseHandler<RemoveChatSubjectUseCase, bool>
{
    /// <summary>
    /// <inheritdoc cref="INotificationsWritableRepository"/>
    /// </summary>
    private readonly INotificationsWritableRepository _repository;

    public RemoveChatSubjectUseCaseHandler(INotificationsWritableRepository repository) =>
        _repository = repository;

    public async Task<Result<bool>> Handle(
        RemoveChatSubjectUseCase useCase,
        CancellationToken ct = default
    )
    {
        long subjectId = useCase.SubjectId;
        Result deleting = await _repository.RemoveGeneralChatSubject(subjectId, ct);
        return deleting.IsFailure ? Result.Failure<bool>(deleting.Error) : true;
    }
}
