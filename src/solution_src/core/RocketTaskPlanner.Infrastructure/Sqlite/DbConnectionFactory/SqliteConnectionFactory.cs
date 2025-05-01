using System.Data;
using Microsoft.Data.Sqlite;
using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.DbConnectionFactory;

public sealed class SqliteConnectionFactory : IDbConnectionFactory
{
    public IDbConnection Create(string connectionString)
    {
        IDbConnection connection = new SqliteConnection(connectionString);
        connection.Open();
        return connection;
    }
}
