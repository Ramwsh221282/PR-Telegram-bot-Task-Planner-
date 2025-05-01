using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetNotificationReceiverTimeInformation;

public sealed record GetNotificationReceiverTimeInformationQueryResponse(
    string Information,
    NotificationReceiverEntity? Entity,
    ApplicationTimeZone? TimeZone
);
