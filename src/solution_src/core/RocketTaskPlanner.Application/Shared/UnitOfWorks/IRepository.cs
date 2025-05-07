using System.Data;

namespace RocketTaskPlanner.Application.Shared.UnitOfWorks;

/// <summary>
/// Контракт для взаимодействия с соединением с БД
/// </summary>
public interface IRepository
{
    /// <summary>
    /// Создание соединения БД
    /// <returns>
    /// <inheritdoc cref="IDbConnection"/>
    /// </returns>
    /// </summary>
    public IDbConnection CreateConnection();
}
