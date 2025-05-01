namespace RocketTaskPlanner.Application.Shared.UseCaseHandler.Decorators;

public interface IMetricsLoggerDecorator<TUseCase, TUseCaseResult>
    : IUseCaseHandler<TUseCase, TUseCaseResult>
    where TUseCase : IUseCase;
