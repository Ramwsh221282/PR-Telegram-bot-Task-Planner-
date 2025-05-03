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
/// <param name="writableRepository">Контракт взаимодействия с БД (операции записи)</param>
public sealed class CreateTaskForChatUseCaseHandler(
    INotificationReceiverWritableRepository writableRepository
) : IUseCaseHandler<CreateTaskForChatUseCase, CreateTaskForChatUseCaseResponse>
{
    private readonly INotificationReceiverWritableRepository _writableRepository =
        writableRepository;

    public async Task<Result<CreateTaskForChatUseCaseResponse>> Handle(
        CreateTaskForChatUseCase useCase,
        CancellationToken ct = default
    )
    {
        if (useCase.DateNotify < useCase.DateCreated)
            return Result.Failure<CreateTaskForChatUseCaseResponse>(
                "Дата вызова меньше даты создания."
            );
        Result<NotificationReceiver> receiver = await _writableRepository.GetById(
            useCase.ChatId,
            ct
        );
        if (receiver.IsFailure)
            return Result.Failure<CreateTaskForChatUseCaseResponse>(receiver.Error);

        ReceiverSubjectId id = ReceiverSubjectId.Create(useCase.SubjectId).Value;
        ReceiverSubjectDateCreated created = new(useCase.DateCreated);
        ReceiverSubjectDateNotify notify = new(useCase.DateNotify);
        ReceiverSubjectTimeInfo time = new(created, notify);
        ReceiverSubjectMessage message = ReceiverSubjectMessage.Create(useCase.Message).Value;
        ReceiverSubjectPeriodInfo periodInfo = new(useCase.isPeriodic);

        GeneralChatReceiverSubject subject = receiver.Value.AddSubject(
            id,
            time,
            periodInfo,
            message
        );
        Result inserting = await _writableRepository.AddSubject(subject, ct);
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
