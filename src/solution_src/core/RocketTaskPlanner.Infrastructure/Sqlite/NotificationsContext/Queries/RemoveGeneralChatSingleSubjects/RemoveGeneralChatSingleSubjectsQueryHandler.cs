using System.Data;
using Dapper;
using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.RemoveGeneralChatSingleSubjects;

public sealed class RemoveGeneralChatSingleSubjectsQueryHandler(IDbConnectionFactory factory)
    : IQueryHandler<RemoveGeneralChatSingleSubjectsQuery, int>
{
    private const string SQL = """
        DELETE FROM general_chat_subjects
        WHERE general_chat_subject_id IN @subjectIds AND general_chat_id = @chatId 
        """;

    private readonly IDbConnectionFactory _factory = factory;

    public async Task<int> Handle(
        RemoveGeneralChatSingleSubjectsQuery query,
        CancellationToken ct = default
    )
    {
        // long[] subjectIds = [.. query.Entities.Select(e => e.GeneralChatSubjectId)];
        // var parameters = new { subjectIds = subjectIds, chatId = query.ChatId };
        // CommandDefinition command = new(SQL, parameters, cancellationToken: ct);
        // IDbConnection connection = _factory.Create(SqliteConstants.NotificationsConnectionString);
        // int removed = await connection.ExecuteAsync(command);
        // return removed;
        throw new NotImplementedException();
    }
}
