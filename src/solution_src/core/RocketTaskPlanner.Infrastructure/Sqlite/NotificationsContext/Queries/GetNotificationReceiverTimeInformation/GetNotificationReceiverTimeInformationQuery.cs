﻿using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetNotificationReceiverTimeInformation;

public sealed record GetNotificationReceiverTimeInformationQuery(long ChatId) : IQuery;
