using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetNotificationReceiverTimeInformation;

/// <summary>
/// Запрос на получение информации о времени <inheritdoc cref="NotificationReceiver"/>
/// <param name="ChatId">ID чата</param>
/// </summary>
public sealed record GetNotificationReceiverTimeInformationQuery(long ChatId) : IQuery;
