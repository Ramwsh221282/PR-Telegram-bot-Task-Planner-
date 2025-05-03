using RocketTaskPlanner.Application.Shared.Validation;
using RocketTaskPlanner.Domain.UsersContext.ValueObjects;

namespace RocketTaskPlanner.Application.UsersContext.Features.AddUserWithPermissions;

public sealed class AddUserWithPermissionUseCaseValidator
    : AbstractValidator<AddUserWithPermissionsUseCase>,
        IValidator<AddUserWithPermissionsUseCase>
{
    public AddUserWithPermissionUseCaseValidator()
    {
        AddValidationRule(
            u => UserName.Create(u.UserName).IsSuccess,
            "Некорректное имя пользователя"
        );
    }
}
