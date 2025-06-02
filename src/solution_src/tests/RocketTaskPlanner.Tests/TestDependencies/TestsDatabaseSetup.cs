using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RocketTaskPlanner.Infrastructure.Database;

namespace RocketTaskPlanner.Tests.TestDependencies;

public static class TestsDatabaseSetup
{
    public static async Task SetupDatabases(this IServiceScope scope)
    {
        await scope.ApplyApplicationTimeDbContext();
    }

    private static async Task ApplyApplicationTimeDbContext(this IServiceScope scope)
    {
        var scopeProvider = scope.ServiceProvider;
        try
        {
            var context = scopeProvider.GetRequiredService<RocketTaskPlannerDbContext>();
            if (!await context.Database.EnsureCreatedAsync())
                await context.Database.MigrateAsync();
        }
        catch
        {
            // ignored
        }
    }
}
