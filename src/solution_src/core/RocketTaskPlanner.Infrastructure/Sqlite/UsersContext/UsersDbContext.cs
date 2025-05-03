using Microsoft.EntityFrameworkCore;
using RocketTaskPlanner.Domain.UsersContext;

namespace RocketTaskPlanner.Infrastructure.Sqlite.UsersContext;

/// <summary>
/// Контекст работы с БД в области пользователей и прав пользователей
/// </summary>
public sealed class UsersDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;

    // конфигурирование db context на использование Sqlite
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(SqliteConstants.UsersConnectionString);
    }

    // Применение настроек таблиц контекста пользователей и прав пользователей
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(UsersDbContext).Assembly,
            t => t.Namespace != null && t.Namespace.Contains("UsersContext")
        );
    }
}
