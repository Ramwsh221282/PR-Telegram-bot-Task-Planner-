using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Queries.GetPageCountForChatSubjects;

/// <summary>
/// Запрос на получение количества страниц по уведомлениям (тем и основных чатов)
/// <param name="ChatId">ID чата</param>
/// <param name="PageSize">Размер страницы</param>
/// <param name="IsPeriodic">Фильтр (периодические или нет)</param>
/// </summary>
public sealed record GetPageCountForChatSubjectsQuery(long ChatId, int PageSize, bool IsPeriodic)
    : IQuery;
