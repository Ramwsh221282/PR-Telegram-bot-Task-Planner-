using RocketTaskPlanner.Application.Shared.Validation;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChat;

/// <summary>
/// Валидатор для <inheritdoc cref="CreateTaskForChatUseCase"/>
/// </summary>
public sealed class CreateTaskForChatUseCaseValidator
    : AbstractValidator<CreateTaskForChatUseCase>,
        IValidator<CreateTaskForChatUseCase>
{
    public CreateTaskForChatUseCaseValidator()
    {
        AddValidationRule(
            v => NotificationReceiverId.Create(v.ChatId).IsSuccess,
            "Некорректный ID чата."
        );
        AddValidationRule(
            v => ReceiverSubjectId.Create(v.SubjectId).IsSuccess,
            "Некорректный ID сообщения."
        );
        AddValidationRule(
            v => ReceiverSubjectMessage.Create(v.Message).IsSuccess,
            "Некорректный текст сообщения"
        );
    }
}
