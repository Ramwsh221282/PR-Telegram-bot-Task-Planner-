using System.Data;
using Npgsql;
using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Database.DbConnectionFactory;

/// <summary>
/// Фабрика создания строки подключения Sqlite
/// </summary>
public sealed class PostgresDbConnectionFactory : IDbConnectionFactory
{
    private readonly DatabaseConfiguration _configuration;
    public PostgresDbConnectionFactory(DatabaseConfiguration configuration) =>
        _configuration = configuration;
    
    /// <summary>
    /// Фабричный метод создания <inheritdoc cref="IDbConnection"/>
    /// <returns>
    /// <inheritdoc cref="IDbConnection"/>
    /// </returns>
    /// </summary>
    public IDbConnection Create()
    {
        string connectionString = _configuration.ConnectionString;
        IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        return connection;
    }
}
