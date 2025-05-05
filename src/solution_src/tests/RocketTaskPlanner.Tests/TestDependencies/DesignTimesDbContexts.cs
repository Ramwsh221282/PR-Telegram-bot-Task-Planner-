using Microsoft.EntityFrameworkCore.Design;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext;
using RocketTaskPlanner.Infrastructure.Sqlite.ExternalChatsManagementContext;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext;

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

public sealed class ExternalChatMembersDesignTimeDbContext
    : IDesignTimeDbContextFactory<ExternalChatsDbContext>
{
    public ExternalChatsDbContext CreateDbContext(string[] args)
    {
        return new ExternalChatsDbContext();
    }
}
