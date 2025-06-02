using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Infrastructure.Env;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Telegram.StartupExtensions.TimeZoneDbSetup;

/// <summary>
/// Utility класс для создания инстанса провайдера временных зон.
/// Токен провайдера читается из переменных окружения, или .env файла.
/// </summary>
public static class TimeZoneDbSetupExtension
{
    /// <summary>
    /// Создать провайдера из переменных окружения. Когда приложение в Production Environment.
    /// </summary>
    public static async Task SetupTimeZoneDbProviderUsingEnvVariables(this IHost host)
    {
        IEnvReader reader = new SystemEnvReader();
        string token = reader.GetEnvironmentVariable("TIME_ZONE_DB_KEY");
        TimeZoneDbProvider provider = CreateProviderInstance(token);
        await host.AddTimeZoneDbProviderInRepository(provider);
    }

    /// <summary>
    /// Создать провайдера временных зон из файла переменных окружения. Когда в Development Environment.
    /// </summary>
    public static async Task SetupTimeZoneDbProviderUsingEnvFile(this IHost host, string filePath)
    {
        IEnvReader reader = new FileEnvReader(filePath);
        string token = reader.GetEnvironmentVariable("TIME_ZONE_DB_KEY");
        TimeZoneDbProvider provider = CreateProviderInstance(token);
        await host.AddTimeZoneDbProviderInRepository(provider);
    }

    /// <summary>
    /// Создание инстанса TimeZoneDbProvider
    /// </summary>
    /// <param name="token">Токен из переменных окружения</param>
    /// <returns>
    ///     <inheritdoc cref="TimeZoneDbProvider"/>
    /// </returns>
    private static TimeZoneDbProvider CreateProviderInstance(string token)
    {
        TimeZoneDbToken tzToken = TimeZoneDbToken.Create(token).Value;
        TimeZoneDbProvider provider = new(tzToken);
        return provider;
    }

    /// <summary>
    /// Пересоздание записи провайдера временных зон в БД.
    /// Используется при перезапуске приложения
    /// </summary>
    private static async Task AddTimeZoneDbProviderInRepository(
        this IHost host,
        TimeZoneDbProvider provider
    )
    {
        var factory = host.Services.GetRequiredService<IServiceScopeFactory>();
        var scope = factory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IApplicationTimeRepository<TimeZoneDbProvider>>();
        await repository.Remove();
        await repository.Add(provider);
        scope.Dispose();
    }
}
