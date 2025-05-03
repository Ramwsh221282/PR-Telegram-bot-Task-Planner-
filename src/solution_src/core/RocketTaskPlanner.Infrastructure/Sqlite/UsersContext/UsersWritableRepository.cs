using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using RocketTaskPlanner.Application.UsersContext.Contracts;
using RocketTaskPlanner.Domain.UsersContext;
using RocketTaskPlanner.Domain.UsersContext.Entities;
using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.UsersContext;

public sealed class UsersWritableRepository(IDbConnectionFactory connectionFactory)
    : IUsersWritableRepository
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;
    private IDbTransaction? _transaction;
    private IDbConnection? _connection;
    private readonly List<Func<IDbConnection, Task<int>>> _operations = [];

    public void BeginTransaction()
    {
        _connection = _connectionFactory.Create(SqliteConstants.UsersConnectionString);
        _transaction = _connection.BeginTransaction();
    }

    public Result<User> AddUser(User user, CancellationToken ct = default)
    {
        if (_transaction == null || _connection == null)
            return Result.Failure<User>("Транзакция не была начата");

        const string addUserSql = """
            INSERT INTO users (id, name)
            VALUES (@id, @name)
            """;

        var parameters = new { id = user.Id.Value, name = user.Name.Value };
        CommandDefinition command = new(addUserSql, parameters, cancellationToken: ct);
        AddComandInExecutionOrder(command);
        return user;
    }

    public Result<UserPermission> AddUserPermission(
        UserPermission permission,
        CancellationToken ct = default
    )
    {
        if (_transaction == null || _connection == null)
            return Result.Failure<UserPermission>("Транзакция не была начата");

        const string addUserPermission = """
            INSERT INTO user_permissions (id, user_id, name)
            VALUES (@id, @user_id, @name)
            """;

        var parameters = new
        {
            id = permission.Id,
            user_id = permission.UserId.Value,
            name = permission.Name,
        };

        CommandDefinition command = new(addUserPermission, parameters, cancellationToken: ct);
        AddComandInExecutionOrder(command);
        return permission;
    }

    public Result<User> RemoveUser(User user, CancellationToken ct = default)
    {
        long userId = user.Id.Value;
        RemoveUser(userId, ct);

        return user;
    }

    public Result<long> RemoveUser(long userId, CancellationToken ct = default)
    {
        const string sql = "DELETE FROM users where id @id; ";

        var parameters = new { id = userId };
        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        AddComandInExecutionOrder(command);

        return userId;
    }

    public async Task<Result> Save()
    {
        if (_transaction == null || _connection == null)
            return Result.Failure("Транзакция не была начата");

        try
        {
            foreach (Func<IDbConnection, Task<int>> operation in _operations)
                await operation(_connection);

            _transaction.Commit();
            return Result.Success();
        }
        catch
        {
            _transaction.Rollback();
            return Result.Failure("Произошла ошибка во время транзакции.");
        }
        finally
        {
            _operations.Clear();
            _transaction.Dispose();
            _connection.Dispose();
        }
    }

    private void AddComandInExecutionOrder(CommandDefinition com) =>
        _operations.Add(conn => conn.ExecuteAsync(com));
}
