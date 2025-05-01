using Microsoft.EntityFrameworkCore;

namespace RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext;

public sealed class ApplicationTimeDbContextFactory : IDbContextFactory<ApplicationTimeDbContext>
{
    public ApplicationTimeDbContext CreateDbContext()
    {
        return new ApplicationTimeDbContext();
    }
}
