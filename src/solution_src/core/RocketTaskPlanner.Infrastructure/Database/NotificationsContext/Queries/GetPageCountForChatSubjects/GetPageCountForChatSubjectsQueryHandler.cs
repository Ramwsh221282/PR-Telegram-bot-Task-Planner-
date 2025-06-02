using System.Data;
using Dapper;
using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Database.Shared;

namespace RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Queries.GetPageCountForChatSubjects;

/// <summary>
/// Обработчик запроса <inheritdoc cref="GetPageCountForChatSubjectsQuery"/>
/// </summary>
public sealed class GetPageCountForChatSubjectsQueryHandler
    : IQueryHandler<GetPageCountForChatSubjectsQuery, int>
{
    /// <summary>
    /// SQL для получения уведомлений из основного чата
    /// </summary>
    private const string _generalChatSql = """
        SELECT
            COUNT(*)
        FROM 
            notification_receivers nr
        LEFT JOIN 
                general_chat_subjects gs ON gs.general_chat_id = nr.receiver_id
        WHERE 
            nr.receiver_id = @chatId AND gs.subject_periodic = @isPeriodic
        """;

    /// <summary>
    /// SQL для получения уведомлений из тем чата
    /// </summary>
    private const string _themeChatSql = """
        SELECT
            COUNT(*)
        FROM 
            receiver_themes rt
        LEFT JOIN 
                theme_chat_subjects rts ON rts.theme_id = rt.theme_id
        WHERE 
            rt.receiver_id = @chatId AND rts.subject_periodic = @isPeriodic
        """;

    /// <summary>
    /// <inheritdoc cref="INotificationsReadableRepository"/>
    /// </summary>
    private readonly IDbConnectionFactory _factory;

    public GetPageCountForChatSubjectsQueryHandler(IDbConnectionFactory factory) =>
        _factory = factory;

    public async Task<int> Handle(
        GetPageCountForChatSubjectsQuery query,
        CancellationToken ct = default
    )
    {
        int pageSize = query.PageSize;
        bool isPeriodic = query.IsPeriodic;
        long chatId = query.ChatId;
        var parameters = new { chatId, isPeriodic };

        using var connection = _factory.Create();

        var generalChatPageCount = await CalculatePagesCountFromGeneralChat(
            parameters,
            connection,
            pageSize,
            ct
        );
        
        var themeChatPageCount = await CalculatePagesCountFromThemeChats(
            parameters,
            connection,
            pageSize,
            ct
        );

        int pagesCount = generalChatPageCount + themeChatPageCount;
        return pagesCount;
    }

    private static async Task<int> CalculatePagesCountFromGeneralChat(
        dynamic parameters,
        IDbConnection connection,
        int limit,
        CancellationToken ct
    )
    {
        var command = new CommandDefinition(_generalChatSql, parameters, cancellationToken: ct);
        return await new TotalPagesCountQuery(command, limit).CalculateTotalPagesCount(connection);
    }

    private static async Task<int> CalculatePagesCountFromThemeChats(
        dynamic parameters,
        IDbConnection connection,
        int limit,
        CancellationToken ct
    )
    {
        var command = new CommandDefinition(_themeChatSql, parameters, cancellationToken: ct);
        return await new TotalPagesCountQuery(command, limit).CalculateTotalPagesCount(connection);
    }
}
