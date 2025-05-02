using RocketTaskPlanner.Application.Shared.Validation;

namespace RocketTaskPlanner.Application.UsersContext.Features.AddUserPermission;

public sealed class AddUserPermissionUseCaseValidator
    : AbstractValidator<AddUserPermissionUseCase>,
        IValidator<AddUserPermissionUseCase>
{
    public AddUserPermissionUseCaseValidator()
    {
        AddValidationRule(
            u => !string.IsNullOrWhiteSpace(u.PermissionName),
            "Некорректное название права"
        );
        AddValidationRule(u => u.PermissionId != Guid.Empty, "Некорректный ID права");
    }
}
