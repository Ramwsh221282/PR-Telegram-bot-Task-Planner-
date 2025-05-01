namespace RocketTaskPlanner.Application.Shared.Validation;

public interface IValidator<TValidatee>
{
    ValidationResult Validate(TValidatee validatee);
}
