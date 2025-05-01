namespace RocketTaskPlanner.Application.Shared.Validation;

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

    public void AddValidationRule(
        Func<TValidatee, bool> validationRuleFactory,
        string errorMessage
    ) => _validationRules.Add(validationRuleFactory, errorMessage);
}
