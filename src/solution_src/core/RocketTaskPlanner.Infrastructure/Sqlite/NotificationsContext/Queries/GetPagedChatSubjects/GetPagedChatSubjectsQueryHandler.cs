using System.Data;
using Dapper;
using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetPagedChatSubjects;

/// <summary>
/// Обработчик запроса <inheritdoc cref="GetPagedChatSubjectsQuery"/>
/// </summary>
public sealed class GetPagedChatSubjectsQueryHandler
    : IQueryHandler<GetPagedChatSubjectsQuery, GetPagedChatSubjectsQueryResponse>
{
    /// <summary>
    /// <inheritdoc cref="INotificationsReadableRepository"/>
    /// </summary>
    private readonly INotificationsReadableRepository _repository;

    public GetPagedChatSubjectsQueryHandler(INotificationsReadableRepository repository) =>
        _repository = repository;

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
            receiver_themes rt
        LEFT JOIN 
                theme_chat_subjects rts ON rts.theme_id = rt.theme_id
        WHERE 
            rt.receiver_id = @chatId AND rts.subject_periodic = @isPeriodic
        LIMIT @limit
        OFFSET @offset;
        """;

    public async Task<GetPagedChatSubjectsQueryResponse> Handle(
        GetPagedChatSubjectsQuery query,
        CancellationToken ct = default
    )
    {
        var limit = query.PageSize;
        var offset = CalculateOffset(query);
        var isPeriodic = FormatIsPeriodicForSqlite(query);
        var chatId = query.ChatId;

        var parameters = new
        {
            chatId,
            isPeriodic,
            limit,
            offset,
        };

        using var connection = _repository.CreateConnection();

        var generalChatTasks = await GetFromGeneralChat(parameters, connection, ct);

        return generalChatTasks.Length > 0
            ? new GetPagedChatSubjectsQueryResponse(generalChatTasks)
            : new GetPagedChatSubjectsQueryResponse(
                await GetFromThemeChats(parameters, chatId, connection, ct)
            );
    }

    private static int FormatIsPeriodicForSqlite(GetPagedChatSubjectsQuery query)
    {
        bool isPeriodic = query.IsPeriodic;
        return isPeriodic ? 1 : 0;
    }

    private static int CalculateOffset(GetPagedChatSubjectsQuery query)
    {
        int page = query.Page;
        int pageSize = query.PageSize;
        int offset = (page - 1) * pageSize;
        return offset;
    }

    private async Task<SubjectsResponse[]> GetFromGeneralChat(
        dynamic parameters,
        IDbConnection connection,
        CancellationToken ct
    )
    {
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
        dynamic parameters,
        long chatId,
        IDbConnection connection,
        CancellationToken ct
    )
    {
        var command = new CommandDefinition(_themeChatSql, parameters, cancellationToken: ct);
        var data = await connection.QueryAsync<ThemeChatSubjectEntity>(command);

        SubjectsResponse[] response =
        [
            .. data.Select(d => new ThemeChatSubjectsQueryResponse(
                chatId,
                d.ThemeId,
                d.ThemeChatSubjectId,
                d.SubjectMessage
            )),
        ];

        return response;
    }
}
