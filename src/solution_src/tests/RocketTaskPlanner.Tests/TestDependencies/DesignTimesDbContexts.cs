using Microsoft.EntityFrameworkCore.Design;
using RocketTaskPlanner.Infrastructure.Database;

namespace RocketTaskPlanner.Tests.TestDependencies;

public sealed class ApplicationTimeDesignTimeDbContext
    : IDesignTimeDbContextFactory<RocketTaskPlannerDbContext>
{
    private readonly DatabaseConfiguration _configuration;

    public ApplicationTimeDesignTimeDbContext(DatabaseConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public RocketTaskPlannerDbContext CreateDbContext(string[] args)
    {
        return new RocketTaskPlannerDbContext(_configuration);
    }
}
