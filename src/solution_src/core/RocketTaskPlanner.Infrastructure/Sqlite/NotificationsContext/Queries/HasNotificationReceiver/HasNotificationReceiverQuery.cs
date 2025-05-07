using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.HasNotificationReceiver;

/// <summary>
/// Запрос на проверку есть ли такой чат
/// <param name="ReceiverId">ID чата</param>
/// </summary>
public sealed record HasNotificationReceiverQuery(long ReceiverId) : IQuery;
