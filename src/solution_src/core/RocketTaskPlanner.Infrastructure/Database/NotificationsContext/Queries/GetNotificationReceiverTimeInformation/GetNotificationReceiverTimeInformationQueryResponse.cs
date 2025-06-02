using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Entities;

namespace RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Queries.GetNotificationReceiverTimeInformation;

/// <summary>
/// Ответ на запрос <inheritdoc cref="GetNotificationReceiverTimeInformationQuery"/>
/// <param name="Information">Информация в виде строки (сообщения)</param>
/// <param name="Entity"><inheritdoc cref="NotificationReceiverEntity"/></param>
/// <param name="TimeZone"><inheritdoc cref="ApplicationTimeZone"/>></param>
/// </summary>
public sealed record GetNotificationReceiverTimeInformationQueryResponse(
    string Information,
    NotificationReceiverEntity? Entity,
    ApplicationTimeZone? TimeZone
);
