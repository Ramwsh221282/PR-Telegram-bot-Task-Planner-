using RocketTaskPlanner.Application.Shared.Validation;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

namespace RocketTaskPlanner.Application.NotificationsContext.Features.RegisterChat;

public sealed class RegisterChatUseCaseValidator
    : AbstractValidator<RegisterChatUseCase>,
        IValidator<RegisterChatUseCase>
{
    public RegisterChatUseCaseValidator()
    {
        AddValidationRule(
            useCase =>
            {
                bool idSuccess = NotificationReceiverId.Create(useCase.ChatId).IsSuccess;
                bool chatNameSuccess = NotificationReceiverName.Create(useCase.ChatName).IsSuccess;
                bool timeZoneSuccess = NotificationReceiverTimeZone
                    .Create(useCase.ZoneName)
                    .IsSuccess;
                return idSuccess && chatNameSuccess && timeZoneSuccess;
            },
            "Некорректные ID и Name чата или некорректный ID темы."
        );
    }
}
