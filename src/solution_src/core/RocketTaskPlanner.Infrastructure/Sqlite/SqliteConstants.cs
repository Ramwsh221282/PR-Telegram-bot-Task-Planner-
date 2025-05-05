namespace RocketTaskPlanner.Infrastructure.Sqlite;

/// <summary>
/// Строки подключения к БД Sqlite
/// </summary>
public static class SqliteConstants
{
    /// <summary>
    /// Строка подключения для контекста чатов и уведомлений
    /// </summary>
    public const string NotificationsConnectionString = "Data Source=Notifications.db";

    /// <summary>
    /// Строка подключения для контекста провайдера времени и временных зон
    /// </summary>
    public const string ApplicationTimeConnectionString = "Data Source=ApplicationTime.db";

    /// <summary>
    /// Строка подключения для контекста пользователей внешних чатов
    /// </summary>
    public const string ExternalChatsConnectionString = "Data Source=ExternalChats.db";
}
