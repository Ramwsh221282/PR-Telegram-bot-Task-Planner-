using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RemoveChatSubject;

public sealed class RemoveChatSubjectUseCaseHandler
    : IUseCaseHandler<RemoveChatSubjectUseCase, bool>
{
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
