using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext;
using RocketTaskPlanner.Infrastructure.Sqlite.PermissionsContext;
using RocketTaskPlanner.Infrastructure.Sqlite.UsersContext;

namespace RocketTaskPlanner.Tests.TestDependencies;

public static class TestsDatabaseDrop
{
    public static async Task DropDatabases()
    {
        await DropNotificationsDatabase();
        await DropUsersDatabase();
        await DropPermissionsDatabase();
        await DropApplicationTimeDatabase();
        DeleteSqliteDbFiles();
    }

    private static void DeleteSqliteDbFiles()
    {
        string directory = AppDomain.CurrentDomain.BaseDirectory;
        DirectoryInfo directoryInfo = new(directory);
        FileInfo[] files = directoryInfo.GetFiles().Where(f => f.Name.EndsWith(".db")).ToArray();
        foreach (FileInfo file in files)
            File.Delete(file.FullName);
    }

    private static async Task DropApplicationTimeDatabase()
    {
        ApplicationTimeDbContext context = new();
        try
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.CloseConnectionAsync();
        }
        catch { }
        await context.DisposeAsync();
    }

    private static async Task DropNotificationsDatabase()
    {
        NotificationsDbContext context = new();
        try
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.CloseConnectionAsync();
        }
        catch { }
        await context.DisposeAsync();
    }

    private static async Task DropPermissionsDatabase()
    {
        PermissionsDbContext context = new();
        try
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.CloseConnectionAsync();
        }
        catch { }
        await context.DisposeAsync();
    }

    private static async Task DropUsersDatabase()
    {
        UsersDbContext context = new();
        try
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.CloseConnectionAsync();
        }
        catch { }
        await context.DisposeAsync();
    }
}
