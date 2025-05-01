using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.HasNotificationReceiverTheme;

public sealed record HasNotificationReceiverThemeQuery(long ReceiverId, long ThemeId) : IQuery;
