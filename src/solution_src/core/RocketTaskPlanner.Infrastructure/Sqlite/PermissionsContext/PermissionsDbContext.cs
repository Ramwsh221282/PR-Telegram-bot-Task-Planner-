using Microsoft.EntityFrameworkCore;
using RocketTaskPlanner.Domain.PermissionsContext;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext;

namespace RocketTaskPlanner.Infrastructure.Sqlite.PermissionsContext;

public sealed class PermissionsDbContext : DbContext
{
    public DbSet<Permission> Permissions { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite(SqliteConstants.PermissionsConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(NotificationsDbContext).Assembly,
            t => t.Namespace != null && t.Namespace.Contains("PermissionsContext")
        );
    }
}
