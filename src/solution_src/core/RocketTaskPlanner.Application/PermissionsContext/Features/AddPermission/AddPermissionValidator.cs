using RocketTaskPlanner.Application.Shared.Validation;

namespace RocketTaskPlanner.Application.PermissionsContext.Features.AddPermission;

public sealed class AddPermissionValidator
    : AbstractValidator<AddPermissionUseCase>,
        IValidator<AddPermissionUseCase>
{
    public AddPermissionValidator()
    {
        AddValidationRule(
            c => !string.IsNullOrEmpty(c.PermissionName),
            "Название права было пустым"
        );
    }
}
