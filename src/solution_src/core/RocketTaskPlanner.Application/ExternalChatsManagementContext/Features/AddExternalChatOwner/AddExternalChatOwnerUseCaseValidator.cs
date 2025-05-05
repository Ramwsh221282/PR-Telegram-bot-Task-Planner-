using RocketTaskPlanner.Application.Shared.Validation;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChatOwner;

public sealed class AddExternalChatOwnerUseCaseValidator
    : AbstractValidator<AddExternalChatOwnerUseCase>,
        IValidator<AddExternalChatOwnerUseCase>
{
    public AddExternalChatOwnerUseCaseValidator()
    {
        AddValidationRule(
            u => ExternalChatMemberName.Create(u.Name).IsSuccess,
            "Некорректное имя владельца чата"
        );
    }
}
