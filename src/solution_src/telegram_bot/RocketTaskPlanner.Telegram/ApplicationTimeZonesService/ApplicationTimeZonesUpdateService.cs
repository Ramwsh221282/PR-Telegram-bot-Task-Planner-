using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Cache;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Telegram.ApplicationTimeZonesService;

/// <summary>
/// Background процесс для обновления временных зон в кеше.
/// </summary>
public sealed class ApplicationTimeZonesUpdateService : BackgroundService
{
    /// <summary>
    /// <inheritdoc cref="TimeZoneDbProviderCachedInstance"/>
    /// </summary>
    private readonly TimeZoneDbProviderCachedInstance _instance;

    /// <summary>
    /// <inheritdoc cref="IApplicationTimeRepository{TProvider}"/>
    /// </summary>
    private readonly IApplicationTimeRepository<TimeZoneDbProvider> _repository;

    /// <summary>
    /// <inheritdoc cref="Serilog.ILogger"/>
    /// </summary>
    private readonly Serilog.ILogger _logger;

    /// <summary>
    /// Название текущего класса
    /// </summary>
    private const string CONTEXT = nameof(ApplicationTimeZonesUpdateService);

    public ApplicationTimeZonesUpdateService(
        TimeZoneDbProviderCachedInstance cachedInstance,
        IApplicationTimeRepository<TimeZoneDbProvider> repository,
        Serilog.ILogger logger
    )
    {
        _logger = logger;
        _logger.Information("{Context} initializng cached instance.", CONTEXT);
        Result<TimeZoneDbProvider> instanceFromDb = repository.Get().Result;

        if (instanceFromDb.IsFailure)
        {
            _instance = cachedInstance;
            _instance.SetNull();
            _logger.Information(
                "{Context} Time Zone Db Provider configuration does not exist. Initialing as none value.",
                CONTEXT
            );
        }
        else
        {
            _instance = cachedInstance;
            _instance.InitializeOrUpdate(instanceFromDb.Value);
            _logger.Information("{Context} Time Zone Db Provider instance initialized.", CONTEXT);
        }
        _repository = repository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.Information("{Context}. Updating time zone db cached instance...", CONTEXT);

            await UpdateApplicationTimeZones(stoppingToken);

            _logger.Information("{Context}. Time Zone Db Instance has been updated...", CONTEXT);

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        _logger.Information("{Context} shut down called.", CONTEXT);
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
