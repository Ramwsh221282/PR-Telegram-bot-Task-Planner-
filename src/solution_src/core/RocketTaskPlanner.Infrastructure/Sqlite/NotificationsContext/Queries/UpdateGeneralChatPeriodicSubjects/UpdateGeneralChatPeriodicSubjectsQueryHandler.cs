using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.UpdateGeneralChatPeriodicSubjects;

public sealed class UpdateGeneralChatPeriodicSubjectsQueryHandler(IDbConnectionFactory factory)
    : IQueryHandler<UpdateGeneralChatPeriodicSubjectsQuery, int>
{
    public async Task<int> Handle(
        UpdateGeneralChatPeriodicSubjectsQuery query,
        CancellationToken ct = default
    )
    {
        throw new NotImplementedException();
    }
}
