using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Infrastructure.Database.ApplicationTimeContext.Cache;
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
    
    private readonly IServiceScopeFactory _scopeFactory;

    /// <summary>
    /// <inheritdoc cref="Serilog.ILogger"/>
    /// </summary>
    private readonly Serilog.ILogger _logger;

    /// <summary>
    /// Название текущего класса
    /// </summary>
    private const string CONTEXT = nameof(ApplicationTimeZonesUpdateService);

    public ApplicationTimeZonesUpdateService(
        IServiceScopeFactory scopeFactory,
        TimeZoneDbProviderCachedInstance cachedInstance,
        Serilog.ILogger logger
    )
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _logger.Information("{Context} initializng cached instance.", CONTEXT);
        
        var scope = _scopeFactory.CreateAsyncScope();
        var scopeProvider = scope.ServiceProvider;
        var repository = scopeProvider.GetRequiredService<IApplicationTimeRepository<TimeZoneDbProvider>>();
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
        
        scope.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.Information("{Context}. Updating time zone db cached instance...", CONTEXT);

                await UpdateApplicationTimeZones(stoppingToken);

                _logger.Information("{Context}. Time Zone Db Instance has been updated...", CONTEXT);

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch(Exception ex)
            {
                _logger.Fatal("{Context} exception: {Exception}", CONTEXT, ex.Message);
            }
        }

        _logger.Information("{Context} shut down called.", CONTEXT);
    }

    private async Task UpdateApplicationTimeZones(CancellationToken cancellationToken)
    {
        TimeZoneDbProvider? provider = _instance.Instance;

        if (provider != null)
        {
            await provider.ProvideTimeZones();

            await using var scope = _scopeFactory.CreateAsyncScope();
            var scopeProvider = scope.ServiceProvider;
            var repository = scopeProvider.GetRequiredService<IApplicationTimeRepository<TimeZoneDbProvider>>();
            await repository.Save(provider, cancellationToken);
        }
    }
}
