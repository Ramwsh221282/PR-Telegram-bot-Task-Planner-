using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;
using RocketTaskPlanner.Utilities.DateExtensions;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChatTheme;

/// <summary>
/// Обработчик для <inheritdoc cref="CreateTaskForChatThemeUseCase"/>
/// </summary>
public sealed class CreateTaskForChatThemeUseCaseHandler
    : IUseCaseHandler<CreateTaskForChatThemeUseCase, CreateTaskForChatThemeUseCaseResponse>
{
    private readonly INotificationsWritableRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskForChatThemeUseCaseHandler(INotificationsWritableRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateTaskForChatThemeUseCaseResponse>> Handle(
        CreateTaskForChatThemeUseCase useCase,
        CancellationToken ct = default
    )
    {
        if (useCase.DateNotify < useCase.DateCreated)
            return Result.Failure<CreateTaskForChatThemeUseCaseResponse>(
                "Дата вызова меньше даты создания."
            );

        var receiver = await _repository.GetById(useCase.ChatId, ct);
        if (receiver.IsFailure) return Result.Failure<CreateTaskForChatThemeUseCaseResponse>(receiver.Error);

        var theme = receiver.Value.Themes.FirstOrDefault(th => th.Id.Id == useCase.ThemeId);
        if (theme == null)
        {
            string error = $"Тема чата {useCase.ChatId} с ID: {useCase.ThemeId} не найдена.";
            return Result.Failure<CreateTaskForChatThemeUseCaseResponse>(error);
        }
        
        await _unitOfWork.BeginTransaction(ct);
        var id = ReceiverSubjectId.Create(useCase.SubjectId).Value;
        var created = new ReceiverSubjectDateCreated(useCase.DateCreated);
        var notify = new ReceiverSubjectDateNotify(useCase.DateNotify);
        var time = new ReceiverSubjectTimeInfo(created, notify);
        var message = ReceiverSubjectMessage.Create(useCase.Message).Value;
        var periodInfo = new ReceiverSubjectPeriodInfo(useCase.isPeriodic);
        var subject = theme.AddSubject(id, time, periodInfo, message);
        
        var savingChanges = await _unitOfWork.SaveChangesAsync(ct);
        if (savingChanges.IsFailure)
        {
            await _unitOfWork.RollBackTransaction(ct);
            return Result.Failure<CreateTaskForChatThemeUseCaseResponse>(savingChanges.Error);
        }
        
        var committing = await _unitOfWork.CommitTransaction(ct);
        
        return committing.IsFailure
            ? Result.Failure<CreateTaskForChatThemeUseCaseResponse>(committing.Error)
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
