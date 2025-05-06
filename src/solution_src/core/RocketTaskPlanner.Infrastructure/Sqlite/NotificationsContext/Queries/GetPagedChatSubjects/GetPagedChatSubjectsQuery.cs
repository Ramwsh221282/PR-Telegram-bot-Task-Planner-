using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetPagedChatSubjects;

public sealed record GetPagedChatSubjectsQuery(long ChatId, int Page, int PageSize, bool IsPeriodic)
    : IQuery;
