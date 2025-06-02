namespace RocketTaskPlanner.Application.Shared.UseCaseHandler.Decorators;

public sealed class GenericExceptionSupressingDecorator<TUseCase, TUseCaseResult>(
    IUseCaseHandler<TUseCase, TUseCaseResult> handler,
    Serilog.ILogger logger
) : IExceptionSupressingDecorator<TUseCase, TUseCaseResult>
    where TUseCase : IUseCase
{
    private readonly Serilog.ILogger _logger = logger;
    private readonly IUseCaseHandler<TUseCase, TUseCaseResult> _handler = handler;

    public async Task<Result<TUseCaseResult>> Handle(
        TUseCase useCase,
        CancellationToken ct = default
    )
    {
        try
        {
            Result<TUseCaseResult> result = await _handler.Handle(useCase, ct);
            return result;
        }
        catch (Exception ex)
        {
            string useCaseName = typeof(TUseCase).Name;
            string exMessage = ex.Message;
            string? exTrace = ex.StackTrace;
            string? exSource = ex.Source;
            _logger.Fatal(
                "Операция: {UseCase}. Исключение: {Ex}. Стак-трейс: {Trace}. Источник: {Source}",
                useCaseName,
                exMessage,
                exTrace,
                exSource
            );
            return Result.Failure<TUseCaseResult>("Операция не выполнена. Внутреннее исключение.");
        }
    }
}
