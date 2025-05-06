using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetPageCountForChatSubjects;

public sealed record GetPageCountForChatSubjectsQuery(long ChatId, int PageSize, bool IsPeriodic)
    : IQuery;
