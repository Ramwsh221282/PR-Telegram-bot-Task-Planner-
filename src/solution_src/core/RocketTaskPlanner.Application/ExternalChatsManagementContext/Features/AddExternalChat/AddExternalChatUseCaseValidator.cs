using RocketTaskPlanner.Application.Shared.Validation;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChat;

/// <summary>
/// Валидатор <inheritdoc cref="AddExternalChatUseCase"/>
/// </summary>
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
