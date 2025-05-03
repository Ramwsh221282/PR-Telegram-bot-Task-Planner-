using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using RocketTaskPlanner.Application.PermissionsContext.Repository;
using RocketTaskPlanner.Domain.PermissionsContext;
using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.PermissionsContext;

/// <summary>
/// Абстракция для работы с БД Прав
/// </summary>
/// <param name="connectionFactory">Фабрика для создания соединений</param>
public sealed class PermissionsWritableRepository(IDbConnectionFactory connectionFactory)
    : IPermissionsWritableRepository
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

    public async Task<Permission> Add(
        Permission permission,
        CancellationToken cancellationToken = default
    )
    {
        const string sql = """
            INSERT INTO Permissions(Id, Name)
            VALUES(@id, @name)
            """;
        var parameters = new { id = permission.Id, name = permission.Name };
        CommandDefinition command = new(sql, parameters, cancellationToken: cancellationToken);
        using IDbConnection connection = _connectionFactory.Create(
            SqliteConstants.PermissionsConnectionString
        );
        await connection.ExecuteAsync(command);
        return permission;
    }

    public async Task<Result<Permission>> GetByName(
        string permissionName,
        CancellationToken cancellationToken = default
    )
    {
        const string sql = """
            SELECT Id, Name FROM Permissions WHERE Name = @name
            """;
        var parameters = new { name = permissionName };
        CommandDefinition command = new(sql, parameters, cancellationToken: cancellationToken);
        using IDbConnection connection = _connectionFactory.Create(
            SqliteConstants.PermissionsConnectionString
        );
        IEnumerable<(Guid, string)> entry = await connection.QueryAsync<(Guid, string)>(command);
        (Guid, string)[] array = [.. entry];
        if (array.Length == 0)
            Result.Failure<Permission>($"Права с названием: {permissionName} не найдены.");
        return new Permission() { Id = array[1].Item1, Name = array[1].Item2 };
    }
}
