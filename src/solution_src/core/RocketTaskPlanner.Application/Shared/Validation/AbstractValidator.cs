namespace RocketTaskPlanner.Application.Shared.Validation;

/// <summary>
/// Класс для валидации объекта TValidatee
/// </summary>
/// <typeparam name="TValidatee">Объект для валидации</typeparam>
public abstract class AbstractValidator<TValidatee> : IValidator<TValidatee>
{
    private readonly Dictionary<Func<TValidatee, bool>, string> _validationRules = [];

    public ValidationResult Validate(TValidatee validatee)
    {
        List<string> _validationErrors = [];
        foreach (KeyValuePair<Func<TValidatee, bool>, string> rule in _validationRules)
        {
            if (!rule.Key(validatee))
                _validationErrors.Add(rule.Value);
        }

        return _validationErrors.Count == 0
            ? ValidationResult.Valid()
            : ValidationResult.NoValid(_validationErrors);
    }

    /// <summary>
    /// Создание валидирующего делегата
    /// </summary>
    /// <param name="validationRuleFactory">Делегат</param>
    /// <param name="errorMessage">Сообщение при ошибке</param>
    public void AddValidationRule(
        Func<TValidatee, bool> validationRuleFactory,
        string errorMessage
    ) => _validationRules.Add(validationRuleFactory, errorMessage);
}
