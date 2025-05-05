using RocketTaskPlanner.Application.Shared.Validation;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChat;

public sealed class AddExternalChatUseCaseValidator
    : AbstractValidator<AddExternalChatUseCase>,
        IValidator<AddExternalChatUseCase>
{
    public AddExternalChatUseCaseValidator()
    {
        AddValidationRule(
            u => ExternalChatName.Create(u.ChatName).IsSuccess,
            "Некорректное название чата"
        );
    }
}
