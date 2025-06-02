using System.Diagnostics;

namespace RocketTaskPlanner.Application.Shared.UseCaseHandler.Decorators;

public sealed class GenericMetricsHandlerDecorator<TUseCase, TUseCaseResult>(
    IUseCaseHandler<TUseCase, TUseCaseResult> handler,
    Serilog.ILogger logger
) : IUseCaseHandler<TUseCase, TUseCaseResult>
    where TUseCase : IUseCase
{
    private readonly IUseCaseHandler<TUseCase, TUseCaseResult> _inner = handler;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<Result<TUseCaseResult>> Handle(
        TUseCase useCase,
        CancellationToken ct = default
    )
    {
        Stopwatch stopwatch = new();

        stopwatch.Start();
        Result<TUseCaseResult> result = await _inner.Handle(useCase, ct);
        stopwatch.Stop();

        double seconds = stopwatch.Elapsed.TotalSeconds;
        string useCaseName = typeof(TUseCase).Name;

        _logger.Information(
            "Операция: {UseCase} выполнение закончилось за: {Seconds} секунд.",
            useCaseName,
            seconds
        );

        return result;
    }
}
