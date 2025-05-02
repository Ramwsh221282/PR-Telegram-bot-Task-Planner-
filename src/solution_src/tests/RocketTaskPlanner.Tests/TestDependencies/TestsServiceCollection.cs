using Microsoft.Extensions.DependencyInjection;
using RocketTaskPlanner.Presenters.DependencyInjection;

namespace RocketTaskPlanner.Tests.TestDependencies;

public static class TestsServiceCollection
{
    public static IServiceCollection BuildServicesCollection()
    {
        ServiceCollection services = new ServiceCollection();
        services.Inject();
        return services;
    }
}
