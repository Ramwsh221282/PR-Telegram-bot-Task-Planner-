using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Infrastructure.Env;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Telegram.StartupExtensions.TimeZoneDbSetup;

public static class TimeZoneDbSetupExtension
{
    public static async Task SetupTimeZoneDbProviderUsingEnvVariables(this IHost host)
    {
        IEnvReader reader = new SystemEnvReader();
        string token = reader.GetEnvironmentVariable("TIME_ZONE_DB_KEY");
        TimeZoneDbProvider provider = CreateProviderInstance(token);
        await host.AddTimeZoneDbProviderInRepository(provider);
    }

    public static async Task SetupTimeZoneDbProviderUsingEnvFile(this IHost host, string filePath)
    {
        IEnvReader reader = new FileEnvReader(filePath);
        string token = reader.GetEnvironmentVariable("TIME_ZONE_DB_KEY");
        TimeZoneDbProvider provider = CreateProviderInstance(token);
        await host.AddTimeZoneDbProviderInRepository(provider);
    }

    private static TimeZoneDbProvider CreateProviderInstance(string token)
    {
        TimeZoneDbToken tzToken = TimeZoneDbToken.Create(token).Value;
        TimeZoneDbProvider provider = new(tzToken);
        return provider;
    }

    private static async Task AddTimeZoneDbProviderInRepository(
        this IHost host,
        TimeZoneDbProvider provider
    )
    {
        var repository = host.Services.GetRequiredService<
            IApplicationTimeRepository<TimeZoneDbProvider>
        >();
        await repository.Remove();
        await repository.Add(provider);
    }
}
