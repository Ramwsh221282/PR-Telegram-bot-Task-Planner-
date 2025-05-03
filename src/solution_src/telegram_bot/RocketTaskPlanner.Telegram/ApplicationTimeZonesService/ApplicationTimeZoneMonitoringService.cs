using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Cache;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Telegram.ApplicationTimeZonesService;

/// <summary>
/// Background процесс для мониторинга временных зон (кеша).
/// </summary>
/// <param name="cachedInstance">Инстанс кеша</param>
/// <param name="logger">Логер</param>
public sealed class ApplicationTimeZoneMonitoringService(
    TimeZoneDbProviderCachedInstance cachedInstance,
    Serilog.ILogger logger
) : BackgroundService
{
    private readonly TimeZoneDbProviderCachedInstance _instance = cachedInstance;
    private readonly Serilog.ILogger _logger = logger;
    private const string CONTEXT = nameof(ApplicationTimeZoneMonitoringService);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.Information("{Context} checking Time Zone Db Provider cache.", CONTEXT);

            TimeZoneDbProvider? providerInstance = _instance.Instance;

            if (providerInstance == null)
                _logger.Information("{Context} cached instance has not been set yet.", CONTEXT);
            else
                _logger.Information("{Context} cached instance is initialized.", CONTEXT);

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        _logger.Information("{Context} shut down called", CONTEXT);
    }
}
