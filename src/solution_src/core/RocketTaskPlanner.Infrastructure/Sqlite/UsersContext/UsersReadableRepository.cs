using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using RocketTaskPlanner.Application.UsersContext.Contracts;
using RocketTaskPlanner.Domain.UsersContext;
using RocketTaskPlanner.Domain.UsersContext.ValueObjects;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.UsersContext.Entities;

namespace RocketTaskPlanner.Infrastructure.Sqlite.UsersContext;

public sealed class UsersReadableRepository(IDbConnectionFactory connectionFactory)
    : IUsersReadableRepository
{
    private readonly IDbConnectionFactory _factory = connectionFactory;

    public async Task<Result<User>> GetById(UserId id, CancellationToken ct = default)
    {
        const string sql = """
            SELECT u.id, u.name, p.id, p.user_id, p.name 
            FROM users u
            LEFT JOIN user_permissions p ON u.id = p.user_id
            WHERE u.id = @id
            """;

        var parameters = new { id = id.Value };
        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        Dictionary<long, UserEntity> entries = [];

        using IDbConnection connection = InstantiateConnection();
        await connection.QueryAsync<UserEntity, UserPermissionEntity, UserEntity>(
            command,
            (user, permission) =>
            {
                if (!entries.TryGetValue(user.Id, out UserEntity? entry))
                {
                    entry = new UserEntity(user);
                    entries.Add(entry.Id, entry);
                }

                if (permission != null)
                    entry.TryAddPermission(permission);

                return entry;
            },
            splitOn: "id"
        );

        return entries.Count == 0
            ? Result.Failure<User>($"Пользователь с ID: {id.Value} не найден.")
            : entries.First().Value.ConvertToUser();
    }

    public async Task<bool> Exists(UserId id, CancellationToken ct = default)
    {
        const string sql = """
            SELECT u.id, u.name 
            FROM users u
            WHERE u.id = @id
            """;

        var parameters = new { id = id.Value };
        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        using IDbConnection connection = InstantiateConnection();
        UserEntity? entries = await connection.QueryFirstOrDefaultAsync<UserEntity>(command);

        return entries != null;
    }

    private IDbConnection InstantiateConnection() =>
        _factory.Create(SqliteConstants.UsersConnectionString);
}
