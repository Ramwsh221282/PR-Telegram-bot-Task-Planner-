using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.RemoveGeneralChatSingleSubjects;

public sealed record RemoveGeneralChatSingleSubjectsQuery(GeneralChatSubjectEntity[] Entities)
    : IQuery;
