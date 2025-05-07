using System.Data;
using Microsoft.Data.Sqlite;
using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.DbConnectionFactory;

/// <summary>
/// Фабрика создания строки подключения Sqlite
/// </summary>
public sealed class SqliteConnectionFactory : IDbConnectionFactory
{
    /// <summary>
    /// Фабричный метод создания <inheritdoc cref="IDbConnection"/>
    /// <param name="connectionString">Строка подключения</param>
    /// <returns>
    /// <inheritdoc cref="IDbConnection"/>
    /// </returns>
    /// </summary>
    public IDbConnection Create(string connectionString)
    {
        IDbConnection connection = new SqliteConnection(connectionString);
        connection.Open();
        return connection;
    }
}
