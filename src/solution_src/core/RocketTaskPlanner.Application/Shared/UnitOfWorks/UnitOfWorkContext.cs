using System.Data;

namespace RocketTaskPlanner.Application.Shared.UnitOfWorks;

/// <summary>
/// Контекст транзакции конкретной БД.
/// </summary>
public sealed class UnitOfWorkContext : IDisposable
{
    /// <summary>
    /// <inheritdoc cref="IDbConnection"/>
    /// </summary>
    public IDbConnection Connection { get; }

    /// <summary>
    /// <inheritdoc cref="IDbTransaction"/>
    /// </summary>
    public IDbTransaction Transaction { get; }

    public UnitOfWorkContext(IDbConnection connection, IDbTransaction transaction)
    {
        Connection = connection;
        Transaction = transaction;
    }

    public override bool Equals(object? obj)
    {
        if (obj is UnitOfWorkContext context)
            return context.Connection.ConnectionString == Connection.ConnectionString;

        return false;
    }

    private bool Equals(UnitOfWorkContext other) =>
        other.Connection.ConnectionString == Connection.ConnectionString;

    public override int GetHashCode() => HashCode.Combine(Connection.ConnectionString);

    public void Dispose()
    {
        Connection.Dispose();
        Transaction.Dispose();
    }
}
