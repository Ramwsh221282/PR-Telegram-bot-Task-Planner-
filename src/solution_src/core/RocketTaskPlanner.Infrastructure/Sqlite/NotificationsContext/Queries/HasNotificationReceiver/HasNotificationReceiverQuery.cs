using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.HasNotificationReceiver;

public sealed record HasNotificationReceiverQuery(long ReceiverId) : IQuery;
