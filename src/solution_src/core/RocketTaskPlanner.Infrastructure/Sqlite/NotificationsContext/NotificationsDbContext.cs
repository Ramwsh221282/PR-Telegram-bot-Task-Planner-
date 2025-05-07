using Microsoft.EntityFrameworkCore;
using RocketTaskPlanner.Domain.NotificationsContext;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext;

/// <summary>
/// Db Context для работы с чатами уведомлений и уведомлениями
/// </summary>
public sealed class NotificationsDbContext : DbContext
{
    /// <summary>
    /// Данные таблицы notification_receivers
    /// </summary>
    public DbSet<NotificationReceiver> Receivers { get; set; } = null!;

    /// <summary>
    /// Конфигурирование <inheritdoc cref="NotificationsDbContext"/>
    /// Использование строки подключения SQLite
    /// <param name="optionsBuilder">
    ///     <inheritdoc cref="optionsBuilder"/>
    /// </param>
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(SqliteConstants.NotificationsConnectionString);
    }

    /// <summary>
    /// Применение настроек для таблицы notification_receivers
    /// <param name="modelBuilder">
    ///     <inheritdoc cref="ModelBuilder"/>
    /// </param>
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(NotificationsDbContext).Assembly,
            t => t.Namespace != null && t.Namespace.Contains("NotificationsContext")
        );
    }
}
