using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetPagedChatSubjects;

/// <summary>
/// Запрос на получение уведомлений постранично
/// <param name="ChatId">
///     ID чата
/// </param>
/// <param name="Page">
///     Текущая страница
/// </param>
/// <param name="PageSize">
///     Размер страницы
/// </param>
/// <param name="IsPeriodic">
///     Фильтр периодичных задач или не периодичных
/// </param>
/// </summary>
public sealed record GetPagedChatSubjectsQuery(long ChatId, int Page, int PageSize, bool IsPeriodic)
    : IQuery;
