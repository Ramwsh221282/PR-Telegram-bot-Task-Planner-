using Microsoft.EntityFrameworkCore;
using RocketTaskPlanner.Domain.UsersContext;

namespace RocketTaskPlanner.Infrastructure.Sqlite.UsersContext;

public sealed class UsersDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(SqliteConstants.UsersConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(UsersDbContext).Assembly,
            t => t.Namespace != null && t.Namespace.Contains("UsersContext")
        );
    }
}
