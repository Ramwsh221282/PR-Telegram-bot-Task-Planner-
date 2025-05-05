using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;
using RocketTaskPlanner.Utilities.DateExtensions;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChat;

/// <summary>
/// Создание уведомления для основного чата
/// </summary>
public sealed class CreateTaskForChatUseCaseHandler
    : IUseCaseHandler<CreateTaskForChatUseCase, CreateTaskForChatUseCaseResponse>
{
    private readonly INotificationRepository _repository;

    public CreateTaskForChatUseCaseHandler(INotificationRepository repository) =>
        _repository = repository;

    public async Task<Result<CreateTaskForChatUseCaseResponse>> Handle(
        CreateTaskForChatUseCase useCase,
        CancellationToken ct = default
    )
    {
        if (useCase.DateNotify < useCase.DateCreated)
            return Result.Failure<CreateTaskForChatUseCaseResponse>(
                "Дата вызова меньше даты создания."
            );

        var receiver = await _repository.Readable.GetById(useCase.ChatId, ct);
        if (receiver.IsFailure)
            return Result.Failure<CreateTaskForChatUseCaseResponse>(receiver.Error);

        var id = ReceiverSubjectId.Create(useCase.SubjectId).Value;
        var created = new ReceiverSubjectDateCreated(useCase.DateCreated);
        var notify = new ReceiverSubjectDateNotify(useCase.DateNotify);
        var time = new ReceiverSubjectTimeInfo(created, notify);
        var message = ReceiverSubjectMessage.Create(useCase.Message).Value;
        var periodInfo = new ReceiverSubjectPeriodInfo(useCase.isPeriodic);
        var subject = receiver.Value.AddSubject(id, time, periodInfo, message);
        Result inserting = await _repository.Writable.AddSubject(subject, ct);

        return inserting.IsFailure
            ? Result.Failure<CreateTaskForChatUseCaseResponse>(receiver.Error)
            : CreateResponse(subject);
    }

    private static CreateTaskForChatUseCaseResponse CreateResponse(
        GeneralChatReceiverSubject subject
    )
    {
        string hasPeriod = subject.Period.IsPeriodic ? "Да" : "Нет";
        string information = $"""
            Информация о созданной задаче:
            Дата создания: {subject.TimeInfo.Created.Value.AsString()}
            Дата вызова: {subject.TimeInfo.Notify.Value.AsString()}
            Повторяется: {hasPeriod}
            """;
        return new CreateTaskForChatUseCaseResponse(information);
    }
}
