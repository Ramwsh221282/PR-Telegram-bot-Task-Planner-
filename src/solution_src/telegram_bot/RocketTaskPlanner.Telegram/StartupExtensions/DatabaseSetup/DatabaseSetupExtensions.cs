using Microsoft.EntityFrameworkCore;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext;
using RocketTaskPlanner.Infrastructure.Sqlite.ExternalChatsManagementContext;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext;

namespace RocketTaskPlanner.Telegram.StartupExtensions.DatabaseSetup;

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
        await scope.ApplyExternalChatMembersDbContext();
    }

    // Создание БД для контекста управления провайдером временных зон.
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

    // Создание БД для контекста уведомлений.
    private static async Task ApplyNotificationDbContext(this AsyncServiceScope scope)
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

    // Контекст для внешних чатов
    private static async Task ApplyExternalChatMembersDbContext(this AsyncServiceScope scope)
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
