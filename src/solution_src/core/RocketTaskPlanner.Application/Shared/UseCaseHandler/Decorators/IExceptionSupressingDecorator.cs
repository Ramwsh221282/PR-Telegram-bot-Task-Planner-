namespace RocketTaskPlanner.Application.Shared.UseCaseHandler.Decorators;

public interface IExceptionSupressingDecorator<TUseCase, TUseCaseResult>
    : IUseCaseHandler<TUseCase, TUseCaseResult>
    where TUseCase : IUseCase;
