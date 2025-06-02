using Microsoft.Extensions.DependencyInjection;
using RocketTaskPlanner.Infrastructure.Database;
using RocketTaskPlanner.Presenters.DependencyInjection;

namespace RocketTaskPlanner.Tests.TestDependencies;

public static class TestsServiceCollection
{
    public static IServiceCollection BuildServicesCollection()
    {
        ServiceCollection services = new ServiceCollection();
        DatabaseConfiguration.AddFromEnvFile(services, DatabaseConfiguration.EnvFile);
        services.InjectApplicationDependencies();
        return services;
    }
}
