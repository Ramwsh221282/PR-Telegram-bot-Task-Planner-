using RocketTaskPlanner.Application.Shared.Validation;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChatTheme;

/// <summary>
/// Валидатор для <inheritdoc cref="CreateTaskForChatThemeUseCase"/>
/// </summary>
public sealed class CreateTaskForChatThemeUseCaseValidator
    : AbstractValidator<CreateTaskForChatThemeUseCase>,
        IValidator<CreateTaskForChatThemeUseCase>
{
    public CreateTaskForChatThemeUseCaseValidator()
    {
        AddValidationRule(
            v => NotificationReceiverId.Create(v.ChatId).IsSuccess,
            "Некорректный ID чата."
        );
        AddValidationRule(
            v => ReceiverThemeId.Create(v.ThemeId).IsSuccess,
            "Некорректный ID темы."
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
