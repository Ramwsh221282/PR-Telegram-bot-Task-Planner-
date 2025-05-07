namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetNotificationReceiversByIdArray;

/// <summary>
/// Ответ запроса <inheritdoc cref="GetNotificationReceiversByIdentifiersQuery"/>
/// <param name="ChatId">ID чата</param>
/// <param name="ChatName">Название чата</param>
/// </summary>
public record GetNotificationReceiversByIdentifiersQueryResponse(long ChatId, string ChatName);
