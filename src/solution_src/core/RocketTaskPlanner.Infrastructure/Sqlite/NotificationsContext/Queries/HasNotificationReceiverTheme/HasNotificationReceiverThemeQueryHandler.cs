using System.Data;
using Dapper;
using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.HasNotificationReceiverTheme;

/// <summary>
/// Обработчик запроса <inheritdoc cref="HasNotificationReceiverThemeQuery"/>
/// <param name="factory">
/// <inheritdoc cref="IDbConnectionFactory"/>
/// </param>
/// </summary>
public sealed class HasNotificationReceiverThemeQueryHandler(IDbConnectionFactory factory)
    : IQueryHandler<HasNotificationReceiverThemeQuery, bool>
{
    private const string SQL = """
        SELECT COUNT(*) FROM receiver_themes
        WHERE theme_id = @theme_id AND receiver_id = @receiver_id
        """;

    private readonly IDbConnectionFactory _factory = factory;

    public async Task<bool> Handle(
        HasNotificationReceiverThemeQuery query,
        CancellationToken ct = default
    )
    {
        long receiverId = query.ReceiverId;
        long themeId = query.ThemeId;
        CommandDefinition command = new(
            SQL,
            new { theme_id = themeId, receiver_id = receiverId },
            cancellationToken: ct
        );
        using IDbConnection connection = _factory.Create(
            SqliteConstants.NotificationsConnectionString
        );
        int count = await connection.ExecuteScalarAsync<int>(command);
        return count != 0;
    }
}
