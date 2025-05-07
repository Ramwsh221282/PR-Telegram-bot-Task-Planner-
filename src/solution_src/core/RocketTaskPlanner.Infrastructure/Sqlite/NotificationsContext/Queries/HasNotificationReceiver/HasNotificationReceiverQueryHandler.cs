using System.Data;
using Dapper;
using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.HasNotificationReceiver;

/// <summary>
/// Обработчик <inheritdoc cref="HasNotificationReceiverQuery"/>
/// <param name="factory"><inheritdoc cref="IDbConnectionFactory"/></param>
/// </summary>
public sealed class HasNotificationReceiverQueryHandler(IDbConnectionFactory factory)
    : IQueryHandler<HasNotificationReceiverQuery, bool>
{
    private readonly IDbConnectionFactory _factory = factory;

    private const string SQL =
        "SELECT COUNT(*) FROM notification_receivers WHERE receiver_id = @id";

    public async Task<bool> Handle(
        HasNotificationReceiverQuery query,
        CancellationToken ct = default
    )
    {
        long receiverId = query.ReceiverId;
        CommandDefinition command = new(SQL, new { id = receiverId }, cancellationToken: ct);
        using IDbConnection connection = _factory.Create(
            SqliteConstants.NotificationsConnectionString
        );
        int count = await connection.ExecuteScalarAsync<int>(command);
        return count != 0;
    }
}
