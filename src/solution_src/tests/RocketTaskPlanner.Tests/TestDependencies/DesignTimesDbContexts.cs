using Microsoft.EntityFrameworkCore.Design;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext;
using RocketTaskPlanner.Infrastructure.Sqlite.PermissionsContext;
using RocketTaskPlanner.Infrastructure.Sqlite.UsersContext;

namespace RocketTaskPlanner.Tests.TestDependencies;

public sealed class ApplicationTimeDesignTimeDbContext
    : IDesignTimeDbContextFactory<ApplicationTimeDbContext>
{
    public ApplicationTimeDbContext CreateDbContext(string[] args)
    {
        return new ApplicationTimeDbContext();
    }
}

public sealed class NotificationsTimeDesignTimeDbContext
    : IDesignTimeDbContextFactory<NotificationsDbContext>
{
    public NotificationsDbContext CreateDbContext(string[] args)
    {
        return new NotificationsDbContext();
    }
}

public sealed class PermissionsDesignTimeDbContext
    : IDesignTimeDbContextFactory<PermissionsDbContext>
{
    public PermissionsDbContext CreateDbContext(string[] args)
    {
        return new PermissionsDbContext();
    }
}

public sealed class UsersDesignTimeDbContext : IDesignTimeDbContextFactory<UsersDbContext>
{
    public UsersDbContext CreateDbContext(string[] args)
    {
        return new UsersDbContext();
    }
}
