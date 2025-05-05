using CSharpFunctionalExtensions;
using Dapper;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.ExternalChatsManagementContext.Entities;

namespace RocketTaskPlanner.Infrastructure.Sqlite.ExternalChatsManagementContext.Repositories;

public sealed class ExternalChatsReadableRepository : IExternalChatsReadableRepository
{
    private readonly IDbConnectionFactory _factory;

    public ExternalChatsReadableRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<Result<ExternalChatOwner>> GetExternalChatOwnerById(
        long id,
        CancellationToken ct = default
    )
    {
        const string sql = """
            SELECT 
            o.id, 
            o.name,
            ec.id,
            ec.parent_id,
            ec.name,
            ec.owner_id
            FROM owners o
            LEFT JOIN external_chats ec ON ec.owner_id = o.id
            WHERE o.id = @id
            """;

        var parameters = new { id };
        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        using var connection = _factory.Create(SqliteConstants.ExternalChatsConnectionString);

        Dictionary<long, ExternalChatOwnerEntity> entries = [];
        await connection.QueryAsync<
            ExternalChatOwnerEntity,
            ExternalChatEntity,
            ExternalChatOwnerEntity
        >(
            command,
            (owner, chat) =>
            {
                if (!entries.TryGetValue(owner.Id, out ExternalChatOwnerEntity? entry))
                {
                    entry = new ExternalChatOwnerEntity(owner);
                    entries.Add(entry.Id, entry);
                }

                if (chat != null)
                    entry.TryAddChat(chat);

                return entry;
            },
            splitOn: "id"
        );

        if (entries.Count == 0)
            return Result.Failure<ExternalChatOwner>($"Не найден обладатель с id: {id}");

        ExternalChatOwner owner = entries.First().Value.ToExternalChatOwner();
        return owner;
    }

    public async Task<bool> IsLastUserChat(long userId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT COUNT(*) FROM external_chats
            WHERE owner_id = @owner_id AND parent_id IS NULL
            """;

        var parameters = new { owner_id = userId };
        var command = new CommandDefinition(sql, parameters, cancellationToken: ct);
        using var connection = _factory.Create(SqliteConstants.ExternalChatsConnectionString);

        int count = await connection.ExecuteScalarAsync<int>(command);
        return count == 1;
    }

    public async Task<bool> ContainsChat(long chatId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT COUNT(*) FROM external_chats
            WHERE id = @id
            """;

        var parameters = new { id = chatId };
        var command = new CommandDefinition(sql, parameters, cancellationToken: ct);
        using var connection = _factory.Create(SqliteConstants.ExternalChatsConnectionString);
        int count = await connection.ExecuteScalarAsync<int>(command);

        return count != 0;
    }

    public async Task<bool> ContainsChildChat(
        long chatId,
        long childChatId,
        CancellationToken ct = default
    )
    {
        const string sql = """
            SELECT COUNT(*) FROM external_chats
            WHERE id = @id AND parent_id = @parentId
            """;

        var parameters = new { id = childChatId, parentId = chatId };
        var command = new CommandDefinition(sql, parameters, cancellationToken: ct);
        using var connection = _factory.Create(SqliteConstants.ExternalChatsConnectionString);
        int count = await connection.ExecuteScalarAsync<int>(command);

        return count != 0;
    }

    public async Task<bool> HasUserRegistered(long userId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT COUNT(*) FROM owners WHERE id = @id
            """;

        var parameters = new { id = userId };
        var command = new CommandDefinition(sql, parameters, cancellationToken: ct);
        using var connection = _factory.Create(SqliteConstants.ExternalChatsConnectionString);
        int count = await connection.ExecuteScalarAsync<int>(command);
        return count != 0;
    }
}
