using System.Text;
using Dapper;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Entities;

namespace RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Queries.GetNotificationReceiversByIdArray;

/// <summary>
/// Обработчик <inheritdoc cref="GetNotificationReceiversByIdentifiersQuery"/>
/// </summary>
public sealed class GetNotificationReceiversByIdentifiersQueryHandler
    : IQueryHandler<
        GetNotificationReceiversByIdentifiersQuery,
        GetNotificationReceiversByIdentifiersQueryResponse[]
    >
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetNotificationReceiversByIdentifiersQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<GetNotificationReceiversByIdentifiersQueryResponse[]> Handle(
        GetNotificationReceiversByIdentifiersQuery query,
        CancellationToken ct = default
    )
    {
        if (query.ParentChatIdentifiers == null || query.ParentChatIdentifiers.Length == 0)
        {
            return [];
        }
        
        StringBuilder builder = new();
        for (int index = 0; index < query.ParentChatIdentifiers.Length; index++)
        {
            if (index <= query.ParentChatIdentifiers.Length - 1)
            {
                builder.Append(query.ParentChatIdentifiers[index]);
                break;
            }
            
            builder.Append($"{query.ParentChatIdentifiers[index]}, ");
        }
        
        string sql = $"""
                           SELECT receiver_id, receiver_name, receiver_zone_name
                           FROM notification_receivers
                           WHERE receiver_id IN ({builder})
                           """;
        
        using var connection = _connectionFactory.Create();
        var command = new CommandDefinition(sql, cancellationToken: ct);
        var chats = await connection.QueryAsync<NotificationReceiverEntity>(command);
        
        var response = chats.Select(c => new GetNotificationReceiversByIdentifiersQueryResponse(
            c.ReceiverId,
            c.ReceiverName
        )).ToArray();
        return response;
    }
}
