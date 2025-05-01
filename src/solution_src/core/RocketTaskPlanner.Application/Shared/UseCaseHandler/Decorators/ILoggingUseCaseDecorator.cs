namespace RocketTaskPlanner.Application.Shared.UseCaseHandler.Decorators;

public interface ILoggingUseCaseDecorator<TUseCase, TUseCaseResult>
    : IUseCaseHandler<TUseCase, TUseCaseResult>
    where TUseCase : IUseCase;
