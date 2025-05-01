using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetNotificationReceiversWithPendingTasks;

public sealed record GetNotificationReceiversWithPendingTasksQuery(
    DateTime ZoneDateTime,
    string ZoneName
) : IQuery;
