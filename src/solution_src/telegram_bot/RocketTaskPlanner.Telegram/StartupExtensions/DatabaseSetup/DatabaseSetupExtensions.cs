using Microsoft.EntityFrameworkCore;
using RocketTaskPlanner.Infrastructure.Database;

namespace RocketTaskPlanner.Telegram.StartupExtensions.DatabaseSetup;

/// <summary>
/// Utility класс для применения миграций и создания локальных БД Sqlite.
/// Все в try catch, поскольку исключения срабатывают, если БД уже есть.
/// </summary>
public static class DatabaseSetupExtensions
{
    public static async Task ApplyMigrations(this IHost host)
    {
        IServiceScopeFactory factory = host.Services.GetRequiredService<IServiceScopeFactory>();
        await using AsyncServiceScope scope = factory.CreateAsyncScope();
        var scopeProvider = scope.ServiceProvider;
        try
        {
            await using var context = scopeProvider.GetRequiredService<RocketTaskPlannerDbContext>();
            if (!await context.Database.EnsureCreatedAsync())
                await context.Database.MigrateAsync();
        }
        catch
        {
            // ignored
        }
    }
}
