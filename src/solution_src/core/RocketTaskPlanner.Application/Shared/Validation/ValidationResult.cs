namespace RocketTaskPlanner.Application.Shared.Validation;

/// <summary>
/// Результат валидации
/// </summary>
/// <param name="IsValid">Валиден или нет</param>
/// <param name="Errors">Ошибки</param>
public sealed record ValidationResult(bool IsValid, List<string> Errors)
{
    public static ValidationResult Valid() => new(true, []);

    public static ValidationResult NoValid(List<string> errors) => new(false, errors);

    public string JoinedErrors(char joiner = '\n') => string.Join(joiner, Errors);
}
