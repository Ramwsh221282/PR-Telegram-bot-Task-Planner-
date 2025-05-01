using Microsoft.EntityFrameworkCore;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext;
using RocketTaskPlanner.Infrastructure.Sqlite.PermissionsContext;

namespace RocketTaskPlanner.Telegram.DatabaseSetup;

public static class DatabaseSetupExtensions
{
    public static async Task ApplyMigrations(this IHost host)
    {
        IServiceScopeFactory factory = host.Services.GetRequiredService<IServiceScopeFactory>();
        await using AsyncServiceScope scope = factory.CreateAsyncScope();
        await scope.ApplyApplicationTimeDbContext();
        await scope.ApplyNotificationDbContext();
        await scope.ApplyPermissionsDbContext();
    }

    private static async Task ApplyApplicationTimeDbContext(this AsyncServiceScope scope)
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

    private static async Task ApplyNotificationDbContext(this AsyncServiceScope scope)
    {
        await using NotificationContextDbContext context =
            scope.ServiceProvider.GetRequiredService<NotificationContextDbContext>();
        try
        {
            if (!await context.Database.EnsureCreatedAsync())
                await context.Database.MigrateAsync();
        }
        catch { }
    }

    private static async Task ApplyPermissionsDbContext(this AsyncServiceScope scope)
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
}
