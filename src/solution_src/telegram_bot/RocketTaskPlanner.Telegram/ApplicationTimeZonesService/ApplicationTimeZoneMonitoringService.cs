using RocketTaskPlanner.Infrastructure.Database.ApplicationTimeContext.Cache;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Telegram.ApplicationTimeZonesService;

/// <summary>
/// Background процесс для мониторинга временных зон (кеша).
/// </summary>
/// <param name="cachedInstance">
///     <inheritdoc cref="TimeZoneDbProviderCachedInstance"/>
/// </param>
/// <param name="logger">
///     <inheritdoc cref="Serilog.ILogger"/>
/// </param>
public sealed class ApplicationTimeZoneMonitoringService(
    TimeZoneDbProviderCachedInstance cachedInstance,
    Serilog.ILogger logger
) : BackgroundService
{
    /// <summary>
    /// <inheritdoc cref="TimeZoneDbProviderCachedInstance"/>
    /// </summary>
    private readonly TimeZoneDbProviderCachedInstance _instance = cachedInstance;

    /// <summary>
    /// <inheritdoc cref="Serilog.ILogger"/>
    /// </summary>
    private readonly Serilog.ILogger _logger = logger;

    /// <summary>
    /// Название текущего класса.
    /// </summary>
    private const string CONTEXT = "Кеш временных зон.";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.Information("{Context}. Проверка кеша.", CONTEXT);

                TimeZoneDbProvider? providerInstance = _instance.Instance;

                if (providerInstance == null)
                    _logger.Information("{Context} Кеш пустой.", CONTEXT);
                else
                    _logger.Information("{Context} Кеш проинициализирован.", CONTEXT);
                _logger.Information("{Context} ожидание 15 секунд.");
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.Fatal("{Context} исключение {Exception}.", CONTEXT, ex.Message);
            }
        }
    }
}
