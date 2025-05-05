using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.Entities;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;
using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.ExternalChatsManagementContext.Repositories;

public sealed class ExternalChatsWritableRepository : IExternalChatsWritableRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ExternalChatsWritableRepository(IDbConnectionFactory connectionFactory) =>
        _connectionFactory = connectionFactory;

    public Result<ExternalChatOwner> AddChatOwner(
        ExternalChatOwner externalChatOwner,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    )
    {
        const string sql = """
            INSERT INTO owners(id, name)
            VALUES(@id, @name)
            """;

        var parameters = new
        {
            id = externalChatOwner.Id.Value,
            name = externalChatOwner.Name.Value,
        };

        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        UnitOfWorkCommand unitCommand = new(async conn => await conn.ExecuteAsync(command));

        unitOfWork.AddCommand(this, unitCommand);
        return externalChatOwner;
    }

    public Result<ExternalChatOwner> RemoveChatOwner(
        ExternalChatOwner externalChatOwner,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    )
    {
        const string sql = "DELETE FROM owners WHERE id = @id";
        var parameters = new { id = externalChatOwner.Id.Value };
        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        UnitOfWorkCommand unitCommand = new(async conn => await conn.ExecuteAsync(command));

        unitOfWork.AddCommand(this, unitCommand);
        return externalChatOwner;
    }

    public Result<ExternalChat> RemoveChat(
        ExternalChat externalChat,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    )
    {
        const string sql = """
            DELETE FROM external_chats
            WHERE id = @id AND owner_id = @owner_id AND parent_id = @id
            """;

        long parentId = externalChat.Id.Value;
        var parameters = new
        {
            id = parentId,
            owner_id = externalChat.OwnerId.Value,
            parent_id = parentId,
        };
        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        UnitOfWorkCommand unitCommand = new(async conn => await conn.ExecuteAsync(command));

        unitOfWork.AddCommand(this, unitCommand);
        return externalChat;
    }

    public Result<ExternalChat> AddChat(
        ExternalChat externalChat,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    )
    {
        const string sql = """
            INSERT INTO external_chats(id, owner_id, name)
            VALUES(@id, @owner_id, @name)
            """;

        var parameters = new
        {
            id = externalChat.Id.Value,
            owner_id = externalChat.OwnerId.Value,
            name = externalChat.Name.Value,
        };

        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        UnitOfWorkCommand unitCommand = new(async conn => await conn.ExecuteAsync(command));

        unitOfWork.AddCommand(this, unitCommand);
        return externalChat;
    }

    public Result<ExternalChat> AddThemeChat(
        ExternalChat externalChat,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    )
    {
        if (externalChat.ParentId == null)
            return Result.Failure<ExternalChat>("Parent Id is null");

        const string sql = """
            INSERT INTO external_chats (id, parent_id, owner_id, name)
            VALUES(@id, @parent_id, @owner_id, @name)
            """;

        var parameters = new
        {
            id = externalChat.Id.Value,
            parent_id = externalChat.ParentId!.Value.Value,
            owner_id = externalChat.OwnerId.Value,
            name = externalChat.Name.Value,
        };

        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        UnitOfWorkCommand unitCommand = new(async conn => await conn.ExecuteAsync(command));
        unitOfWork.AddCommand(this, unitCommand);

        return externalChat;
    }

    public Result<ExternalChat> RemoveThemeChat(
        ExternalChat externalChat,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    )
    {
        if (externalChat.ParentId == null)
            return Result.Failure<ExternalChat>("Parent Id is null");

        const string sql = """
            DELETE FROM external_chats WHERE id = @id AND parent_id = @parent_id AND owner_id = @owner_id
            """;

        var parameters = new
        {
            id = externalChat.Id.Value,
            parent_id = externalChat.ParentId.Value.Value,
            owner_id = externalChat.OwnerId.Value,
        };

        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        UnitOfWorkCommand unitCommand = new(async conn => await conn.ExecuteAsync(command));
        unitOfWork.AddCommand(this, unitCommand);

        return externalChat;
    }

    public IDbConnection CreateConnection() =>
        _connectionFactory.Create(SqliteConstants.ExternalChatsConnectionString);
}
