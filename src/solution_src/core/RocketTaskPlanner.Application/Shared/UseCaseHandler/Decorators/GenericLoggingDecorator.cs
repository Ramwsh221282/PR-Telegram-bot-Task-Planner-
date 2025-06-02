namespace RocketTaskPlanner.Application.Shared.UseCaseHandler.Decorators;

public sealed class GenericLoggingDecorator<TUseCase, TUseCaseResult>(
    IUseCaseHandler<TUseCase, TUseCaseResult> handler,
    Serilog.ILogger logger
) : ILoggingUseCaseDecorator<TUseCase, TUseCaseResult>
    where TUseCase : IUseCase
{
    private readonly IUseCaseHandler<TUseCase, TUseCaseResult> _handler = handler;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<Result<TUseCaseResult>> Handle(
        TUseCase useCase,
        CancellationToken ct = default
    )
    {
        Result<TUseCaseResult> result = await _handler.Handle(useCase, ct);
        string useCaseName = typeof(TUseCase).Name;
        if (result.IsFailure)
        {
            string error = result.Error;
            _logger.Error("Операция: {UseCase} обработана. Ошибка: {Error}", useCaseName, error);
        }
        if (result.IsSuccess)
        {
            string info = GetUseCaseInfo(useCase);
            _logger.Information(
                "Операция: {UseCase} обработана. Информация: {Info}",
                useCaseName,
                info
            );
        }
        return result;
    }

    private string GetUseCaseInfo(TUseCase useCase)
    {
        string? info = useCase.ToString();
        return string.IsNullOrWhiteSpace(info) ? "Информации нет" : info;
    }
}
