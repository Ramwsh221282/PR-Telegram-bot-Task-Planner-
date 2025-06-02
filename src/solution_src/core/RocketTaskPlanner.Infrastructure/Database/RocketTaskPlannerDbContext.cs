using Microsoft.EntityFrameworkCore;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Infrastructure.Database;

public sealed class RocketTaskPlannerDbContext : DbContext
{
    private readonly DatabaseConfiguration _configuration;
    
    public DbSet<TimeZoneDbProvider> Providers => Set<TimeZoneDbProvider>();
    public DbSet<ExternalChatOwner> Owners => Set<ExternalChatOwner>();
    public DbSet<NotificationReceiver> Receivers => Set<NotificationReceiver>();

    public RocketTaskPlannerDbContext(DatabaseConfiguration configuration)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseNpgsql(_configuration.ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RocketTaskPlannerDbContext).Assembly);
}