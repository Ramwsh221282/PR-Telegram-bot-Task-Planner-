using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Cache;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Telegram.ApplicationTimeZonesService;

public sealed class ApplicationTimeZonesUpdateService : BackgroundService
{
    private readonly TimeZoneDbProviderCachedInstance _instance;
    private readonly IApplicationTimeRepository<TimeZoneDbProvider> _repository;
    private readonly Serilog.ILogger _logger;

    public ApplicationTimeZonesUpdateService(
        TimeZoneDbProviderCachedInstance cachedInstance,
        IApplicationTimeRepository<TimeZoneDbProvider> repository,
        Serilog.ILogger logger
    )
    {
        _logger = logger;
        _logger.Information(
            "{Context} initializng cached instance.",
            nameof(ApplicationTimeZonesUpdateService)
        );
        Result<TimeZoneDbProvider> instanceFromDb = repository.Get().Result;
        if (instanceFromDb.IsFailure)
        {
            _instance = cachedInstance;
            _instance.SetNull();
            _logger.Information(
                "{Context} Time Zone Db Provider configuration does not exist. Initialing as none value.",
                nameof(ApplicationTimeZonesUpdateService)
            );
        }
        else
        {
            _instance = cachedInstance;
            _instance.InitializeOrUpdate(instanceFromDb.Value);
            _logger.Information(
                "{Context} Time Zone Db Provider instance initialized.",
                nameof(ApplicationTimeZonesUpdateService)
            );
        }
        _repository = repository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.Information(
                "{Context}. Updating time zone db cached instance...",
                nameof(ApplicationTimeZonesUpdateService)
            );
            await UpdateApplicationTimeZones(stoppingToken);
            _logger.Information(
                "{Context}. Time Zone Db Instance has been updated...",
                nameof(ApplicationTimeZonesUpdateService)
            );
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
        _logger.Information(
            "{Context} shut down called.",
            nameof(ApplicationTimeZonesUpdateService)
        );
    }

    private async Task UpdateApplicationTimeZones(CancellationToken cancellationToken)
    {
        TimeZoneDbProvider? provider = _instance.Instance;
        if (provider != null)
        {
            await provider.ProvideTimeZones();
            await _repository.Save(provider, cancellationToken);
        }
    }
}
