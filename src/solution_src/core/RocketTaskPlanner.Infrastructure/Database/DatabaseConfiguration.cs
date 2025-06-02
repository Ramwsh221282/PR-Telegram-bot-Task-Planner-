using Microsoft.Extensions.DependencyInjection;
using RocketTaskPlanner.Infrastructure.Env;

namespace RocketTaskPlanner.Infrastructure.Database;

/// <summary>
/// Строки подключения к БД Sqlite
/// </summary>
public sealed class DatabaseConfiguration
{
    public static readonly string EnvFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env");
    public string ConnectionString { get; private set; } = string.Empty;

    public static void AddFromEnvFile(IServiceCollection services, string filePath)
    {
        IEnvReader reader = new FileEnvReader(filePath);
        string user = reader.GetEnvironmentVariable("DB_USER");
        string password = reader.GetEnvironmentVariable("DB_PASSWORD");
        string port = reader.GetEnvironmentVariable("DB_PORT");
        string name = reader.GetEnvironmentVariable("DB_NAME");
        string host = reader.GetEnvironmentVariable("DB_HOST");
        string connectionString =
            $"Host={host};Port={port};Username={user};Password={password};Database={name}";
        DatabaseConfiguration configuration = new()
        {
            ConnectionString = connectionString
        };
        services.AddSingleton(configuration);
    }

    public static void AddFromEnvironmentVariables(IServiceCollection services)
    {
        IEnvReader reader = new SystemEnvReader();
        string user = reader.GetEnvironmentVariable("DB_USER");
        string password = reader.GetEnvironmentVariable("DB_PASSWORD");
        string port = reader.GetEnvironmentVariable("DB_PORT");
        string name = reader.GetEnvironmentVariable("DB_NAME");
        string host = reader.GetEnvironmentVariable("DB_HOST");
        string connectionString =
            $"Host={host};Port={port};Username={user};Password={password};Database={name}";
        DatabaseConfiguration configuration = new()
        {
            ConnectionString = connectionString
        };
        services.AddSingleton(configuration);
    }
}
