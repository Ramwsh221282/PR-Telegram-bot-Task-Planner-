using RocketTaskPlanner.Application.Shared.Validation;

namespace RocketTaskPlanner.Application.Shared.UseCaseHandler.Decorators;

public sealed class GenericValidationDecorator<TUseCase, TUseCaseResult>(
    IUseCaseHandler<TUseCase, TUseCaseResult> handler,
    IValidator<TUseCase> validator
) : IValidatingUseCaseDecorator<TUseCase, TUseCaseResult>
    where TUseCase : IUseCase
{
    private readonly IValidator<TUseCase> _validator = validator;
    private readonly IUseCaseHandler<TUseCase, TUseCaseResult> _handler = handler;

    public async Task<Result<TUseCaseResult>> Handle(
        TUseCase useCase,
        CancellationToken ct = default
    )
    {
        ValidationResult validation = _validator.Validate(useCase);
        return validation.IsValid == false
            ? Result.Failure<TUseCaseResult>(validation.JoinedErrors())
            : await _handler.Handle(useCase, ct);
    }
}
