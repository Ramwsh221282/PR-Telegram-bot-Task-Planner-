using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.HasNotificationReceiverTheme;

/// <summary>
/// Запрос на проверку существования темы
/// <param name="ReceiverId">ID чата</param>
/// <param name="ThemeId">ID темы</param>
/// </summary>
public sealed record HasNotificationReceiverThemeQuery(long ReceiverId, long ThemeId) : IQuery;
