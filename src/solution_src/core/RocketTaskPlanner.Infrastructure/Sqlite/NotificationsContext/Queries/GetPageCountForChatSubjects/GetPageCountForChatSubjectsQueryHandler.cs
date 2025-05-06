using System.Data;
using Dapper;
using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.Shared;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetPageCountForChatSubjects;

public sealed class GetPageCountForChatSubjectsQueryHandler
    : IQueryHandler<GetPageCountForChatSubjectsQuery, int>
{
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

    private readonly INotificationsReadableRepository _repository;

    public GetPageCountForChatSubjectsQueryHandler(INotificationsReadableRepository repository) =>
        _repository = repository;

    public async Task<int> Handle(
        GetPageCountForChatSubjectsQuery query,
        CancellationToken ct = default
    )
    {
        int pageSize = query.PageSize;
        int isPeriodic = query.IsPeriodic ? 1 : 0;
        long chatId = query.ChatId;
        var parameters = new { chatId, isPeriodic };

        using var connection = _repository.CreateConnection();

        var generalChatPageCountTask = CalculatePagesCountFromGeneralChat(
            parameters,
            connection,
            pageSize,
            ct
        );
        var themeChatPageCountTask = CalculatePagesCountFromThemeChats(
            parameters,
            connection,
            pageSize,
            ct
        );

        await Task.WhenAll(generalChatPageCountTask, themeChatPageCountTask);

        var generalChatPageCount = await generalChatPageCountTask;
        var themeChatPageCount = await themeChatPageCountTask;

        int pagesCount = generalChatPageCount + themeChatPageCount;
        return pagesCount;
    }

    private async Task<int> CalculatePagesCountFromGeneralChat(
        dynamic parameters,
        IDbConnection connection,
        int limit,
        CancellationToken ct
    )
    {
        var command = new CommandDefinition(_generalChatSql, parameters, cancellationToken: ct);
        return await new TotalPagesCountQuery(command, limit).CalculateTotalPagesCount(connection);
    }

    private async Task<int> CalculatePagesCountFromThemeChats(
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
