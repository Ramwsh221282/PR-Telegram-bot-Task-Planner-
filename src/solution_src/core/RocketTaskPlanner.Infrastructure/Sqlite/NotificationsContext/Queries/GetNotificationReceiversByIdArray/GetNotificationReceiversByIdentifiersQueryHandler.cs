using Dapper;
using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetNotificationReceiversByIdArray;

public sealed class GetNotificationReceiversByIdentifiersQueryHandler
    : IQueryHandler<
        GetNotificationReceiversByIdentifiersQuery,
        GetNotificationReceiversByIdentifiersQueryResponse[]
    >
{
    private readonly INotificationsReadableRepository _repository;

    private const string _sql = """
        SELECT receiver_id, receiver_name, receiver_zone_name
        FROM notification_receivers
        WHERE receiver_id IN @chatIds
        """;

    public GetNotificationReceiversByIdentifiersQueryHandler(
        INotificationsReadableRepository repository
    ) => _repository = repository;

    public async Task<GetNotificationReceiversByIdentifiersQueryResponse[]> Handle(
        GetNotificationReceiversByIdentifiersQuery query,
        CancellationToken ct = default
    )
    {
        var parameters = new { chatIds = query.ChatIdentifiers };
        var command = new CommandDefinition(_sql, parameters, cancellationToken: ct);
        var connection = _repository.CreateConnection();
        var chats = await connection.QueryAsync<NotificationReceiverEntity>(command);

        GetNotificationReceiversByIdentifiersQueryResponse[] response =
        [
            .. chats.Select(c => new GetNotificationReceiversByIdentifiersQueryResponse(
                c.ReceiverId,
                c.ReceiverName
            )),
        ];

        return response;
    }
}
