using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetNotificationReceiversByIdArray;

/// <summary>
/// Dto получения основных чатов по списку ID этих чатов
/// <param name="ChatIdentifiers">Список ID чатов</param>
/// </summary>
public sealed record GetNotificationReceiversByIdentifiersQuery(long[] ChatIdentifiers) : IQuery;
