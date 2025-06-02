using Microsoft.EntityFrameworkCore;

namespace RocketTaskPlanner.Infrastructure.Database;

public sealed class RocketTaskPlannerDbContextFactory : IDbContextFactory<RocketTaskPlannerDbContext>
{
    private readonly DatabaseConfiguration _configuration;

    public RocketTaskPlannerDbContextFactory(DatabaseConfiguration configuration) => _configuration = configuration;
    
    public RocketTaskPlannerDbContext CreateDbContext() => new(_configuration);
}