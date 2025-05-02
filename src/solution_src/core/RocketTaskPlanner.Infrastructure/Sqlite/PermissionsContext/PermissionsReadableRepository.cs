using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using RocketTaskPlanner.Application.PermissionsContext.Repository;
using RocketTaskPlanner.Domain.PermissionsContext;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.PermissionsContext.Entities;

namespace RocketTaskPlanner.Infrastructure.Sqlite.PermissionsContext;

public sealed class PermissionsReadableRepository : IPermissionsReadableRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PermissionsReadableRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<Permission>> GetByName(
        string permissionName,
        CancellationToken ct = default
    )
    {
        const string sql = """
            SELECT id, name
            FROM permissions
            WHERE name = @name;
            """;

        var parameters = new { name = permissionName };
        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        using IDbConnection connection = InstantiateConnection();
        PermissionEntity? permission = await connection.QueryFirstOrDefaultAsync<PermissionEntity>(
            command
        );

        return permission?.ConvertToPermission()
            ?? Result.Failure<Permission>($"Права с названием: {permissionName} не найдены.");
    }

    public async Task<bool> Contains(string permissionName, CancellationToken ct = default)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM permissions
            WHERE name = @name;
            """;

        var parameters = new { name = permissionName };
        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        using IDbConnection connection = InstantiateConnection();
        int count = await connection.ExecuteScalarAsync<int>(command);

        return count != 0;
    }

    private IDbConnection InstantiateConnection()
    {
        return _connectionFactory.Create(SqliteConstants.PermissionsConnectionString);
    }
}
