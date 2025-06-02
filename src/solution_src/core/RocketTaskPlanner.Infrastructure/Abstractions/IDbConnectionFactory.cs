using System.Data;

namespace RocketTaskPlanner.Infrastructure.Abstractions;

/// <summary>
/// Контракт создания соединения с БД
/// </summary>
public interface IDbConnectionFactory
{
    IDbConnection Create();
}
