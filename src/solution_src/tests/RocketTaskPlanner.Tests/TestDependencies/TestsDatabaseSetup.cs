using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext;
using RocketTaskPlanner.Infrastructure.Sqlite.PermissionsContext;
using RocketTaskPlanner.Infrastructure.Sqlite.UsersContext;

namespace RocketTaskPlanner.Tests.TestDependencies;

public static class TestsDatabaseSetup
{
    public static async Task SetupDatabases(this IServiceScope scope)
    {
        await scope.ApplyNotificationDbContext();
        await scope.ApplyUsersDbContext();
        await scope.ApplyPermissionsDbContext();
        await scope.ApplyApplicationTimeDbContext();
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

    private static async Task ApplyPermissionsDbContext(this IServiceScope scope)
    {
        await using PermissionsDbContext context =
            scope.ServiceProvider.GetRequiredService<PermissionsDbContext>();
        try
        {
            if (!await context.Database.EnsureCreatedAsync())
                await context.Database.MigrateAsync();
        }
        catch { }
    }

    private static async Task ApplyUsersDbContext(this IServiceScope scope)
    {
        await using UsersDbContext context =
            scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        try
        {
            if (!await context.Database.EnsureCreatedAsync())
                await context.Database.MigrateAsync();
        }
        catch { }
    }
}
