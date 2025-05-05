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

    public TaskFromRemovedChat(long chatId) => _chatId = chatId;

    public async Task<ITaskToFire> Fire() => await Task.FromResult(this);

    public async Task HandleTaskFromRemovedChat(IDbConnectionFactory factory)
    {
        using IDbConnection externalChatsConnection = factory.Create(
            SqliteConstants.ExternalChatsConnectionString
        );
        long ownerId = await GetOwnerIdOfUnsubscribedChat(externalChatsConnection);
        await DeleteUnsubscribedExternalChatOwner(externalChatsConnection, ownerId);
        using IDbConnection notificationsConnection = factory.Create(
            SqliteConstants.NotificationsConnectionString
        );
        await DeleteChatAsNotificationReceiver(notificationsConnection);
    }

    private async Task DeleteChatAsNotificationReceiver(IDbConnection connection)
    {
        const string sql = "DELETE FROM notification_receivers WHERE receiver_id = @id;";
        var parameters = new { id = _chatId };
        CommandDefinition command = new(sql, parameters);
        await connection.ExecuteAsync(command);
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
}
