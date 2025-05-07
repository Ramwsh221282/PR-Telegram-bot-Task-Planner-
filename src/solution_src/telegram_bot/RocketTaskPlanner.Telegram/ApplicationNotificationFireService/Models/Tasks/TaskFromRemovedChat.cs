using System.Data;
using Dapper;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks;

/// <summary>
/// Класс для удаления чата и обладателя чата, если он удалил бота из чата телеграм.
/// </summary>
public sealed class TaskFromRemovedChat : ITaskToFire
{
    private readonly long _chatId;
    private readonly long _subjectId;

    public TaskFromRemovedChat(long chatId, long subjectId)
    {
        _chatId = chatId;
        _subjectId = subjectId;
    }

    public async Task<ITaskToFire> Fire() => await Task.FromResult(this);

    public async Task HandleTaskFromRemovedChat(IDbConnectionFactory factory)
    {
        await TryRemoveOwner(factory);

        if (await DeleteChatAsNotificationReceiver(factory))
            return;

        if (await TryDeleteAsGeneralChatSubject(_subjectId, factory))
            return;

        await TryDeleteAsThemeChatSubject(factory, _subjectId);
    }

    private async Task TryRemoveOwner(IDbConnectionFactory factory)
    {
        const string connectionString = SqliteConstants.ExternalChatsConnectionString;
        using var connection = factory.Create(connectionString);
        try
        {
            long ownerId = await GetOwnerIdOfUnsubscribedChat(connection);
            await DeleteUnsubscribedExternalChatOwner(connection, ownerId);
        }
        catch { }
    }

    private async Task<bool> DeleteChatAsNotificationReceiver(IDbConnectionFactory factory)
    {
        const string connectionString = SqliteConstants.NotificationsConnectionString;
        var connection = factory.Create(connectionString);
        try
        {
            const string sql = "DELETE FROM notification_receivers WHERE receiver_id = @id;";
            var parameters = new { id = _chatId };
            CommandDefinition command = new(sql, parameters);
            await connection.ExecuteAsync(command);
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            connection.Dispose();
        }
    }

    private async Task<long> GetOwnerIdOfUnsubscribedChat(IDbConnection connection)
    {
        const string sql = "SELECT owner_id FROM external_chats WHERE id = @id";
        var parameters = new { id = _chatId };
        CommandDefinition command = new(sql, parameters);
        return await connection.QueryFirstAsync<long>(command);
    }

    private static async Task DeleteUnsubscribedExternalChatOwner(
        IDbConnection connection,
        long ownerId
    )
    {
        const string sql = "DELETE FROM owners WHERE id = @id";
        var parameters = new { id = ownerId };
        CommandDefinition command = new(sql, parameters);
        await connection.ExecuteAsync(command);
    }

    private static async Task<bool> TryDeleteAsGeneralChatSubject(
        long subjectId,
        IDbConnectionFactory factory
    )
    {
        const string sql =
            "DELETE FROM general_chat_subjects WHERE general_chat_subject_id = @subjectId";
        var parameters = new { subjectId };
        const string connectionString = SqliteConstants.NotificationsConnectionString;
        var command = new CommandDefinition(sql, parameters);
        var connection = factory.Create(connectionString);
        try
        {
            await connection.ExecuteAsync(command);
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            connection.Dispose();
        }
    }

    private static async Task<bool> TryDeleteAsThemeChatSubject(
        IDbConnectionFactory factory,
        long subjectId
    )
    {
        const string sql =
            "DELETE FROM theme_chat_subjects WHERE theme_chat_subject_id = @subjectId";
        var parameters = new { subjectId };
        const string connectionString = SqliteConstants.NotificationsConnectionString;
        var connection = factory.Create(connectionString);
        var command = new CommandDefinition(sql, parameters);
        try
        {
            await connection.ExecuteAsync(command);
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            connection.Dispose();
        }
    }
}
