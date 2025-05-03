using Microsoft.EntityFrameworkCore;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext;
using RocketTaskPlanner.Infrastructure.Sqlite.PermissionsContext;
using RocketTaskPlanner.Infrastructure.Sqlite.UsersContext;

namespace RocketTaskPlanner.Telegram.DatabaseSetup;

/// <summary>
/// Utility класс для применения миграций и создания локальных БД Sqlite
/// </summary>
public static class DatabaseSetupExtensions
{
    public static async Task ApplyMigrations(this IHost host)
    {
        IServiceScopeFactory factory = host.Services.GetRequiredService<IServiceScopeFactory>();
        await using AsyncServiceScope scope = factory.CreateAsyncScope();
        await scope.ApplyApplicationTimeDbContext();
        await scope.ApplyNotificationDbContext();
        await scope.ApplyPermissionsDbContext();
        await scope.ApplyUsersDbContext();
    }

    // Создание БД для контекста управления провайдером временных зон.
    private static async Task ApplyApplicationTimeDbContext(this AsyncServiceScope scope)
    {
        await using ApplicationTimeDbContext context =
            scope.ServiceProvider.GetRequiredService<ApplicationTimeDbContext>();
        if (!await context.Database.EnsureCreatedAsync())
            await context.Database.MigrateAsync();
    }

    // Создание БД для контекста уведомлений.
    private static async Task ApplyNotificationDbContext(this AsyncServiceScope scope)
    {
        await using NotificationsDbContext context =
            scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        if (!await context.Database.EnsureCreatedAsync())
            await context.Database.MigrateAsync();
    }

    // Создание БД для контекста прав
    private static async Task ApplyPermissionsDbContext(this AsyncServiceScope scope)
    {
        await using PermissionsDbContext context =
            scope.ServiceProvider.GetRequiredService<PermissionsDbContext>();
        if (!await context.Database.EnsureCreatedAsync())
            await context.Database.MigrateAsync();
    }

    // Создание БД для контекста пользователей
    private static async Task ApplyUsersDbContext(this AsyncServiceScope scope)
    {
        await using UsersDbContext context =
            scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        if (!await context.Database.EnsureCreatedAsync())
            await context.Database.MigrateAsync();
    }
}
