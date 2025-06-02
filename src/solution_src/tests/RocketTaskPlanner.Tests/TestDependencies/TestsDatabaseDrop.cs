using Microsoft.Extensions.DependencyInjection;
using RocketTaskPlanner.Infrastructure.Database;

namespace RocketTaskPlanner.Tests.TestDependencies;

public static class TestsDatabaseDrop
{
    public static async Task DropDatabases(this IServiceScope scope)
    {
        await scope.DropDatabase();
    }
    
    private static async Task DropDatabase(this IServiceScope scope)
    {
        var provider = scope.ServiceProvider;
        try
        {
            await using var context = provider.GetRequiredService<RocketTaskPlannerDbContext>();
            await context.Database.EnsureDeletedAsync();
        }
        catch
        {
            // ignored
        }
    }
}
