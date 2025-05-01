using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Cache;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Telegram.ApplicationTimeZonesService;

public sealed class ApplicationTimeZoneMonitoringService(
    TimeZoneDbProviderCachedInstance cachedInstance,
    Serilog.ILogger logger
) : BackgroundService
{
    private readonly TimeZoneDbProviderCachedInstance _instance = cachedInstance;
    private readonly Serilog.ILogger _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.Information(
                "{Context} monitoring cached Time Zone Db Provider instance.",
                nameof(ApplicationTimeZoneMonitoringService)
            );
            TimeZoneDbProvider? providerInstance = _instance.Instance;
            if (providerInstance == null)
            {
                _logger.Information(
                    "{Context} cached instance has not been set yet.",
                    nameof(ApplicationTimeZoneMonitoringService)
                );
            }
            else
            {
                _logger.Information(
                    "{Context} cached instance is initialized.",
                    nameof(ApplicationTimeZoneMonitoringService)
                );
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        _logger.Information(
            "{Context} shut down called",
            nameof(ApplicationTimeZoneMonitoringService)
        );
    }
}
