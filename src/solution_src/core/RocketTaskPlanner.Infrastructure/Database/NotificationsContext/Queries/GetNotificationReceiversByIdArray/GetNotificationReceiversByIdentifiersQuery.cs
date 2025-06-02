using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Queries.GetNotificationReceiversByIdArray;

/// <summary>
/// Dto получения основных чатов по списку ID этих чатов
/// <param name="ParentChatIdentifiers">Список ID чатов</param>
/// </summary>
public sealed record GetNotificationReceiversByIdentifiersQuery(long[] ParentChatIdentifiers) : IQuery;
