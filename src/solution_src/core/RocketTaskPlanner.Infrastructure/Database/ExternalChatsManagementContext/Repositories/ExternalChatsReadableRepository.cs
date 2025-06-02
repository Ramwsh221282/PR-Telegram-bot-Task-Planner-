using CSharpFunctionalExtensions;
using Dapper;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Database.ExternalChatsManagementContext.Entities;

namespace RocketTaskPlanner.Infrastructure.Database.ExternalChatsManagementContext.Repositories;

/// <summary>
/// Абстракция взаимодействия БД пользователей и пользовательскими чатами (операции чтения)
/// </summary>
public sealed class ExternalChatsReadableRepository : IExternalChatsReadableRepository
{
    /// <summary>
    /// <inheritdoc cref="IDbConnectionFactory"/>
    /// </summary>
    private readonly IDbConnectionFactory _factory;

    public ExternalChatsReadableRepository(IDbConnectionFactory factory) => _factory = factory;

    /// <summary>
    /// Получить <inheritdoc cref="ExternalChatOwner"/> по его ID
    /// <param name="id">ID</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Result Success если найден с таким ID, либо Failure</returns>
    /// </summary>
    public async Task<Result<ExternalChatOwner>> GetExternalChatOwnerById(long id, CancellationToken ct = default)
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
        using var connection = _factory.Create();
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

    /// <summary>
    /// Проверка на то, последний ли чат у пользователя или нет
    /// <param name="userId">ID пользователя</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>True если у пользователя остался 1 чат. Иначе false</returns>
    /// </summary>
    public async Task<bool> IsLastUserChat(long userId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT COUNT(*) FROM external_chats
            WHERE owner_id = @owner_id AND parent_id IS NULL
            """;

        var parameters = new { owner_id = userId };
        var command = new CommandDefinition(sql, parameters, cancellationToken: ct);
        using var connection = _factory.Create();

        int count = await connection.ExecuteScalarAsync<int>(command);
        return count == 1;
    }

    /// <summary>
    /// Проверка на существование чата
    /// <param name="chatId">ID чата</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>True если чат существует. Иначе false</returns>
    /// </summary>
    public async Task<bool> ContainsChat(long chatId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT COUNT(*) FROM external_chats
            WHERE id = @id
            """;

        var parameters = new { id = chatId };
        var command = new CommandDefinition(sql, parameters, cancellationToken: ct);
        using var connection = _factory.Create();
        int count = await connection.ExecuteScalarAsync<int>(command);

        return count != 0;
    }

    /// <summary>
    /// Проверка на существование дочернего чата (темы)
    /// <param name="chatId">ИД родителя</param>
    /// <param name="childChatId">ИД дочернего чата</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>True если дочерний чат существует. Иначе false</returns>
    /// </summary>
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
        using var connection = _factory.Create();
        int count = await connection.ExecuteScalarAsync<int>(command);

        return count != 0;
    }

    /// <summary>
    /// Проверка на существования пользователя с таким ID
    /// <param name="userId">ID пользователей</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>True если пользователь найден, иначе False.</returns>
    /// </summary>
    public async Task<bool> HasUserRegistered(long userId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT COUNT(*) FROM owners WHERE id = @id
            """;

        var parameters = new { id = userId };
        var command = new CommandDefinition(sql, parameters, cancellationToken: ct);
        using var connection = _factory.Create();
        int count = await connection.ExecuteScalarAsync<int>(command);
        return count != 0;
    }

    /// <summary>
    /// Проверка на то, является ли пользователем владельцем чата
    /// <param name="userId">ID пользователя</param>
    /// <param name="chatId">ID чата</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>True если владеет, иначе False</returns>
    /// </summary>
    public async Task<bool> UserOwnsChat(long userId, long chatId, CancellationToken ct = default)
    {
        const string sql = """
             SELECT COUNT(*) FROM external_chats WHERE id = @chatId AND owner_id = @userId
            """;

        var parameters = new { chatId, userId };
        var command = new CommandDefinition(sql, parameters, cancellationToken: ct);
        using var connection = _factory.Create();
        int count = await connection.ExecuteScalarAsync<int>(command);
        return count != 0;
    }
}
