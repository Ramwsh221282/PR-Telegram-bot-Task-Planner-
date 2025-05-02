using RocketTaskPlanner.Application.Shared.Validation;
using RocketTaskPlanner.Domain.UsersContext.ValueObjects;

namespace RocketTaskPlanner.Application.UsersContext.Features.AddUser;

public sealed class AddUserUseCaseValidator
    : AbstractValidator<AddUserUseCase>,
        IValidator<AddUserUseCase>
{
    public AddUserUseCaseValidator()
    {
        AddValidationRule(
            u => UserName.Create(u.Username).IsSuccess,
            "Некорректное имя пользователя"
        );
    }
}
