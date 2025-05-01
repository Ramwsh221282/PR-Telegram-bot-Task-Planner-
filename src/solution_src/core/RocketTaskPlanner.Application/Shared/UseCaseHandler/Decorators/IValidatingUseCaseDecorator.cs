namespace RocketTaskPlanner.Application.Shared.UseCaseHandler.Decorators;

public interface IValidatingUseCaseDecorator<TUseCase, TUseCaseResult>
    : IUseCaseHandler<TUseCase, TUseCaseResult>
    where TUseCase : IUseCase;
