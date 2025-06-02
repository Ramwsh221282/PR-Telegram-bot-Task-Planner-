using Dapper;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Entities;

namespace RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Queries.GetReceiverThemesByParentId;

public sealed record GetNotificationReceiverThemesByParentId(long ParentId) : IQuery;

public sealed class
    GetNotificationReceiverThemesByParentIdHandler 
    : IQueryHandler<GetNotificationReceiverThemesByParentId, ReceiverThemeEntity[]>
{
    private const string Sql =
        """
        SELECT * from receiver_themes r
        WHERE r.receiver_id = @receiverId
        """;

    private readonly IDbConnectionFactory _factory;

    public GetNotificationReceiverThemesByParentIdHandler(IDbConnectionFactory factory)
    {
        _factory = factory;
    }
    
    public async Task<ReceiverThemeEntity[]> Handle(GetNotificationReceiverThemesByParentId query, CancellationToken ct = default)
    {
        var parameters = new { receiverId = query.ParentId };
        var command = new CommandDefinition(Sql, parameters, cancellationToken: ct);
        using var connection = _factory.Create();
        var themes = await connection.QueryAsync<ReceiverThemeEntity>(command);
        return themes.ToArray();
    }
}