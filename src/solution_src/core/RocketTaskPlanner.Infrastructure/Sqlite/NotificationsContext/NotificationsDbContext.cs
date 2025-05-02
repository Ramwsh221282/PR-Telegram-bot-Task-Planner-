using Microsoft.EntityFrameworkCore;
using RocketTaskPlanner.Domain.NotificationsContext;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext;

public sealed class NotificationsDbContext : DbContext
{
    public DbSet<NotificationReceiver> Receivers { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(SqliteConstants.NotificationsConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(NotificationsDbContext).Assembly,
            t => t.Namespace != null && t.Namespace.Contains("NotificationsContext")
        );
    }
}
