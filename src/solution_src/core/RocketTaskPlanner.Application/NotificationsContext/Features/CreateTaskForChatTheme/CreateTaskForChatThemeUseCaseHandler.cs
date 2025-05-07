using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;
using RocketTaskPlanner.Utilities.DateExtensions;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChatTheme;

/// <summary>
/// Обработчик для <inheritdoc cref="CreateTaskForChatThemeUseCase"/>
/// </summary>
public sealed class CreateTaskForChatThemeUseCaseHandler
    : IUseCaseHandler<CreateTaskForChatThemeUseCase, CreateTaskForChatThemeUseCaseResponse>
{
    /// <summary>
    /// <inheritdoc cref="INotificationRepository"/>
    /// </summary>
    private readonly INotificationRepository _repository;

    public CreateTaskForChatThemeUseCaseHandler(INotificationRepository repository) =>
        _repository = repository;

    public async Task<Result<CreateTaskForChatThemeUseCaseResponse>> Handle(
        CreateTaskForChatThemeUseCase useCase,
        CancellationToken ct = default
    )
    {
        if (useCase.DateNotify < useCase.DateCreated)
            return Result.Failure<CreateTaskForChatThemeUseCaseResponse>(
                "Дата вызова меньше даты создания."
            );

        var receiver = await _repository.Readable.GetById(useCase.ChatId, ct);
        if (receiver.IsFailure)
            return Result.Failure<CreateTaskForChatThemeUseCaseResponse>(receiver.Error);

        var theme = receiver.Value.Themes.FirstOrDefault(th => th.Id.Id == useCase.ThemeId);
        if (theme == null)
            return Result.Failure<CreateTaskForChatThemeUseCaseResponse>(
                $"Тема чата {useCase.ChatId} с ID: {useCase.ThemeId} не найдена."
            );

        var id = ReceiverSubjectId.Create(useCase.SubjectId).Value;
        var created = new ReceiverSubjectDateCreated(useCase.DateCreated);
        var notify = new ReceiverSubjectDateNotify(useCase.DateNotify);
        var time = new ReceiverSubjectTimeInfo(created, notify);
        var message = ReceiverSubjectMessage.Create(useCase.Message).Value;
        var periodInfo = new ReceiverSubjectPeriodInfo(useCase.isPeriodic);
        var subject = theme.AddSubject(id, time, periodInfo, message);

        var inserting = await _repository.Writable.AddSubject(subject, ct);
        return inserting.IsFailure
            ? Result.Failure<CreateTaskForChatThemeUseCaseResponse>(receiver.Error)
            : CreateResponse(subject);
    }

    private static CreateTaskForChatThemeUseCaseResponse CreateResponse(ThemeChatSubject subject)
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
