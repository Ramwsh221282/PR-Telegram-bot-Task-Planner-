using Microsoft.EntityFrameworkCore;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext;

public sealed class ApplicationTimeDbContext : DbContext
{
    public DbSet<TimeZoneDbProvider> Providers { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(SqliteConstants.ApplicationTimeConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationTimeDbContext).Assembly,
            t => t.Namespace != null && t.Namespace.Contains("ApplicationTimeContext")
        );
    }
}
