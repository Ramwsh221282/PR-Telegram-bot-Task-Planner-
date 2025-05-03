namespace RocketTaskPlanner.Application.Shared.Validation;

/// <summary>
/// Интерфейс валидатора
/// </summary>
/// <typeparam name="TValidatee">Объект который нужно валидировать</typeparam>
public interface IValidator<TValidatee>
{
    ValidationResult Validate(TValidatee validatee);
}
