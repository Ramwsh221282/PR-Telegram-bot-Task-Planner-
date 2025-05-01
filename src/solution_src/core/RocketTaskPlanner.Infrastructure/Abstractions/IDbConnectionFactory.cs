using System.Data;

namespace RocketTaskPlanner.Infrastructure.Abstractions;

public interface IDbConnectionFactory
{
    IDbConnection Create(string connectionString);
}
