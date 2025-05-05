using Microsoft.EntityFrameworkCore;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext;

namespace RocketTaskPlanner.Infrastructure.Sqlite.ExternalChatsManagementContext;

/// <summary>
/// Контекст работы с базой данных контекста Обладателя внешнего чата и участников внешнего чата.
/// </summary>
public sealed class ExternalChatsDbContext : DbContext
{
    public DbSet<ExternalChatOwner> Owners { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite(SqliteConstants.ExternalChatsConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ExternalChatsDbContext).Assembly,
            t => t.Namespace != null && t.Namespace.Contains("ExternalChatsManagementContext")
        );
}
