using RocketTaskPlanner.Application.Shared.Validation;

namespace RocketTaskPlanner.Application.Shared.UseCaseHandler.Decorators;

public sealed class GenericValidationDecorator<TUseCase, TUseCaseResult>(
    IUseCaseHandler<TUseCase, TUseCaseResult> handler,
    IEnumerable<IValidator<TUseCase>> validators
) : IValidatingUseCaseDecorator<TUseCase, TUseCaseResult>
    where TUseCase : IUseCase
{
    private readonly IEnumerable<IValidator<TUseCase>> _validators = validators;
    private readonly IUseCaseHandler<TUseCase, TUseCaseResult> _handler = handler;

    public async Task<Result<TUseCaseResult>> Handle(
        TUseCase useCase,
        CancellationToken ct = default
    )
    {
        foreach (var validator in _validators)
        {
            ValidationResult validation = validator.Validate(useCase);
            if (validation.IsValid == false)
                return Result.Failure<TUseCaseResult>(validation.JoinedErrors());
        }
        return await _handler.Handle(useCase, ct);
    }
}
