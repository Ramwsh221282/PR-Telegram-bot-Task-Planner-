using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetNotificationReceiversByIdArray;

public sealed record GetNotificationReceiversByIdentifiersQuery(long[] ChatIdentifiers) : IQuery;
