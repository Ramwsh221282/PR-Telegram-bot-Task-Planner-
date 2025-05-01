using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.UpdateGeneralChatPeriodicSubjects;

public sealed record UpdateGeneralChatPeriodicSubjectsQuery(GeneralChatSubjectEntity[] Entities)
    : IQuery;
