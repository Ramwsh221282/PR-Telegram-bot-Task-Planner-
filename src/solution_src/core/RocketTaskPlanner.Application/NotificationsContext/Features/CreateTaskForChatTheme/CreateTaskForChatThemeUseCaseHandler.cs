using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;
using RocketTaskPlanner.Utilities.DateExtensions;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChatTheme;

/// <summary>
/// Создание уведомления для темы чата
/// </summary>
/// <param name="writableRepository">Контракт взаимодействия с БД (операции записи)</param>
public sealed class CreateTaskForChatThemeUseCaseHandler(
    INotificationReceiverWritableRepository writableRepository
) : IUseCaseHandler<CreateTaskForChatThemeUseCase, CreateTaskForChatThemeUseCaseResponse>
{
    private readonly INotificationReceiverWritableRepository _writableRepository =
        writableRepository;

    public async Task<Result<CreateTaskForChatThemeUseCaseResponse>> Handle(
        CreateTaskForChatThemeUseCase useCase,
        CancellationToken ct = default
    )
    {
        if (useCase.DateNotify < useCase.DateCreated)
            return Result.Failure<CreateTaskForChatThemeUseCaseResponse>(
                "Дата вызова меньше даты создания."
            );
        Result<NotificationReceiver> receiver = await _writableRepository.GetById(
            useCase.ChatId,
            ct
        );
        if (receiver.IsFailure)
            return Result.Failure<CreateTaskForChatThemeUseCaseResponse>(receiver.Error);

        ReceiverTheme? theme = receiver.Value.Themes.FirstOrDefault(th =>
            th.Id.Id == useCase.ThemeId
        );
        if (theme == null)
            return Result.Failure<CreateTaskForChatThemeUseCaseResponse>(
                $"Тема чата {useCase.ChatId} с ID: {useCase.ThemeId} не найдена."
            );
        ReceiverSubjectId id = ReceiverSubjectId.Create(useCase.SubjectId).Value;
        ReceiverSubjectDateCreated created = new(useCase.DateCreated);
        ReceiverSubjectDateNotify notify = new(useCase.DateNotify);
        ReceiverSubjectTimeInfo time = new(created, notify);
        ReceiverSubjectMessage message = ReceiverSubjectMessage.Create(useCase.Message).Value;
        ReceiverSubjectPeriodInfo periodInfo = new(useCase.isPeriodic);
        ThemeChatSubject subject = theme.AddSubject(id, time, periodInfo, message);
        Result inserting = await _writableRepository.AddSubject(subject, ct);
        return inserting.IsFailure
            ? Result.Failure<CreateTaskForChatThemeUseCaseResponse>(receiver.Error)
            : CreateResponse(subject);
    }

    private CreateTaskForChatThemeUseCaseResponse CreateResponse(ThemeChatSubject subject)
    {
        string hasPeriod = subject.Period.IsPeriodic ? "Да" : "Нет";
        string information = $"""
            Информация о созданной задаче:
            Дата создания: {subject.TimeInfo.Created.Value.AsString()}
            Дата вызова: {subject.TimeInfo.Notify.Value.AsString()}
            Повторяется: {hasPeriod}
            """;
        return new CreateTaskForChatThemeUseCaseResponse(information);
    }
}
