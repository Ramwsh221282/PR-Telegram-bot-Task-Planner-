using RocketTaskPlanner.Application.Shared.Validation;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RegisterTheme;

public sealed class RegisterThemeUseCaseValidator
    : AbstractValidator<RegisterThemeUseCase>,
        IValidator<RegisterThemeUseCase>
{
    public RegisterThemeUseCaseValidator()
    {
        AddValidationRule(
            useCase =>
            {
                bool chatIdSuccess = NotificationReceiverId.Create(useCase.ChatId).IsSuccess;
                bool themeIdSuccess = ReceiverThemeId.Create(useCase.ThemeId).IsSuccess;
                return chatIdSuccess && themeIdSuccess;
            },
            "ID название чата некорректны, либо ID темы некорректен."
        );
    }
}
