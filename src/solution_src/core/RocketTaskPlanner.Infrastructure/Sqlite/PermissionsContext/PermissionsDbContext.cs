using Microsoft.EntityFrameworkCore;
using RocketTaskPlanner.Domain.PermissionsContext;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext;

namespace RocketTaskPlanner.Infrastructure.Sqlite.PermissionsContext;

/// <summary>
/// Контекст для работы с БД правами
/// </summary>
public sealed class PermissionsDbContext : DbContext
{
    public DbSet<Permission> Permissions { get; set; } = null!;

    // настройка работы контекста с Sqlite
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite(SqliteConstants.PermissionsConnectionString);

    // применение настроек таблиц контекста прав
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(NotificationsDbContext).Assembly,
            t => t.Namespace != null && t.Namespace.Contains("PermissionsContext")
        );
    }
}
