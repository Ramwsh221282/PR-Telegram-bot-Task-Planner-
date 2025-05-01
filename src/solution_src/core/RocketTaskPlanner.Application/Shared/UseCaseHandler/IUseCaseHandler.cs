namespace RocketTaskPlanner.Application.Shared.UseCaseHandler;

public interface IUseCaseHandler<TUseCase, TUseCaseResult>
    where TUseCase : IUseCase
{
    Task<Result<TUseCaseResult>> Handle(TUseCase useCase, CancellationToken ct = default);
}

public interface IUseCaseHandler<TUseCase>
    where TUseCase : IUseCase
{
    Task<Result> Handle(TUseCase useCase, CancellationToken ct);
}
