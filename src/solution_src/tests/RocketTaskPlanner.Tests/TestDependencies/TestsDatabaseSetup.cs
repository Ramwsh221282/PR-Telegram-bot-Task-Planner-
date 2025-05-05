using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext;
using RocketTaskPlanner.Infrastructure.Sqlite.ExternalChatsManagementContext;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext;

namespace RocketTaskPlanner.Tests.TestDependencies;

public static class TestsDatabaseSetup
{
    public static async Task SetupDatabases(this IServiceScope scope)
    {
        await scope.ApplyNotificationDbContext();
        await scope.ApplyApplicationTimeDbContext();
        await scope.ApplyExternalChatsDbContext();
    }

    private static async Task ApplyApplicationTimeDbContext(this IServiceScope scope)
    {
        await using ApplicationTimeDbContext context =
            scope.ServiceProvider.GetRequiredService<ApplicationTimeDbContext>();
        try
        {
            if (!await context.Database.EnsureCreatedAsync())
                await context.Database.MigrateAsync();
        }
        catch { }
    }

    private static async Task ApplyNotificationDbContext(this IServiceScope scope)
    {
        await using NotificationsDbContext context =
            scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        try
        {
            if (!await context.Database.EnsureCreatedAsync())
                await context.Database.MigrateAsync();
        }
        catch { }
    }

    private static async Task ApplyExternalChatsDbContext(this IServiceScope scope)
    {
        await using ExternalChatsDbContext context =
            scope.ServiceProvider.GetRequiredService<ExternalChatsDbContext>();
        try
        {
            if (!await context.Database.EnsureCreatedAsync())
                await context.Database.MigrateAsync();
        }
        catch { }
    }
}
