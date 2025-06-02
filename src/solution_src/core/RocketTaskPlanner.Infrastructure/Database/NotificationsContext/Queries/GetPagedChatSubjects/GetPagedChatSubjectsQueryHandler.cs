using System.Data;
using Dapper;
using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Entities;

namespace RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Queries.GetPagedChatSubjects;

/// <summary>
/// Обработчик запроса <inheritdoc cref="GetPagedChatSubjectsQuery"/>
/// </summary>
public sealed class GetPagedChatSubjectsQueryHandler
    : IQueryHandler<GetPagedChatSubjectsQuery, GetPagedChatSubjectsQueryResponse>
{
    /// <summary>
    /// <inheritdoc cref="INotificationsReadableRepository"/>
    /// </summary>
    private readonly IDbConnectionFactory _factory;

    public GetPagedChatSubjectsQueryHandler(IDbConnectionFactory factory) =>
        _factory = factory;

    /// <summary>
    /// SQL для основного чата
    /// </summary>
    private const string _generalChatSql = """
        SELECT
          gs.general_chat_subject_id,
          gs.general_chat_id,
          gs.subject_periodic,
          gs.subject_created,
          gs.subject_notify,
          gs.subject_message
        FROM 
            notification_receivers nr
        LEFT JOIN 
                general_chat_subjects gs ON gs.general_chat_id = nr.receiver_id
        WHERE 
            nr.receiver_id = @chatId AND gs.subject_periodic = @isPeriodic
        LIMIT @limit
        OFFSET @offset;
        """;

    /// <summary>
    /// SQL для тем чата
    /// </summary>
    private const string _themeChatSql = """
        SELECT 
            rts.theme_chat_subject_id,
            rts.theme_id,
            rts.subject_periodic,
            rts.subject_created,
            rts.subject_notify,
            rts.subject_message
        FROM 
            theme_chat_subjects rts
        LEFT JOIN 
                receiver_themes rt ON rts.theme_id = rt.theme_id
        WHERE 
            rt.receiver_id = @mainChatId AND rts.subject_periodic = @isPeriodic AND rt.theme_id = @themeId
        LIMIT @limit
        OFFSET @offset
        """;

    public async Task<GetPagedChatSubjectsQueryResponse> Handle(
        GetPagedChatSubjectsQuery query,
        CancellationToken ct = default
    )
    {
        var isTheme = query.IsTheme;
        using var connection = _factory.Create();
        return isTheme
            ? new GetPagedChatSubjectsQueryResponse(await GetFromThemeChats(query, connection, ct))
            : new GetPagedChatSubjectsQueryResponse(await GetFromGeneralChat(query, connection, ct));
    }

    private static bool FormatIsPeriodicForSqlite(GetPagedChatSubjectsQuery query)
    {
        return query.IsPeriodic;
    }

    private static int CalculateOffset(GetPagedChatSubjectsQuery query)
    {
        int page = query.Page;
        int pageSize = query.PageSize;
        int offset = (page - 1) * pageSize;
        return offset;
    }

    private async Task<SubjectsResponse[]> GetFromGeneralChat(
        GetPagedChatSubjectsQuery query,
        IDbConnection connection,
        CancellationToken ct
    )
    {
        var parameters = new
        {
            chatId = query.ChatId,
            isPeriodic = query.IsPeriodic,
            limit = query.PageSize,
            offset = CalculateOffset(query),
        };
        
        var command = new CommandDefinition(_generalChatSql, parameters, cancellationToken: ct);
        var data = await connection.QueryAsync<GeneralChatSubjectEntity>(command);

        SubjectsResponse[] response =
        [
            .. data.Select(d => new GeneralChatSubjectsQueryResponse(
                d.GeneralChatId,
                d.GeneralChatSubjectId,
                d.SubjectMessage
            )),
        ];
        return response;
    }

    private async Task<SubjectsResponse[]> GetFromThemeChats(
        GetPagedChatSubjectsQuery query,
        IDbConnection connection,
        CancellationToken ct
    )
    {
    
        // SELECT 
        // rts.theme_chat_subject_id,
        // rts.theme_id,
        // rts.subject_periodic,
        // rts.subject_created,
        // rts.subject_notify,
        // rts.subject_message
        //     FROM 
        // theme_chat_subjects rts
        // LEFT JOIN 
        // receiver_themes rt ON rts.theme_id = rt.theme_id
        // WHERE 
        // rt.receiver_id = @mainChatId AND rts.subject_periodic = @isPeriodic AND rt.theme_id = @themeId
        // LIMIT @limit
        // OFFSET @offset
        
        var parameters = new
        {
            themeId = query.ChatId,
            isPeriodic = query.IsPeriodic,
            limit = query.PageSize,
            offset = CalculateOffset(query),
            mainChatId = query.ParentId!.Value
        };
        
        var command = new CommandDefinition(_themeChatSql, parameters, cancellationToken: ct);
        var data = await connection.QueryAsync<ThemeChatSubjectEntity>(command);
        
        SubjectsResponse[] response =
        [
            .. data.Select(d => new ThemeChatSubjectsQueryResponse(
                query.ChatId,
                d.ThemeId,
                d.ThemeChatSubjectId,
                d.SubjectMessage
            )),
        ];

        return response;
    }
}
