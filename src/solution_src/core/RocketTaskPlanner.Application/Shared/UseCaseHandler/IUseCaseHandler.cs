namespace RocketTaskPlanner.Application.Shared.UseCaseHandler;

/// <summary>
/// Интерфейс обработчика команды (CQRS)
/// </summary>
/// <typeparam name="TUseCase">Команда</typeparam>
/// <typeparam name="TUseCaseResult">Результат команды</typeparam>
public interface IUseCaseHandler<TUseCase, TUseCaseResult>
    where TUseCase : IUseCase
{
    Task<Result<TUseCaseResult>> Handle(TUseCase useCase, CancellationToken ct = default);
}
