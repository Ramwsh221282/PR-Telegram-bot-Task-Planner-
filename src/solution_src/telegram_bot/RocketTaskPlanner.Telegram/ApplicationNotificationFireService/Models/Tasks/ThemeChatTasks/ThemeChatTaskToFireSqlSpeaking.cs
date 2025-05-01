using System.Data;
using Dapper;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks.ThemeChatTasks;

public sealed class ThemeChatTaskToFireSqlSpeaking : IThemeChatTaskToFire
{
    private const string _sql = """
        DELETE FROM theme_chat_subjects
        WHERE
        theme_chat_subject_id = @subjectId
        AND
        theme_id = @themeId
        """;
    private readonly IThemeChatTaskToFire _task;
    private readonly IDbConnectionFactory _connectionFactory;

    public ThemeChatTaskToFireSqlSpeaking(
        IThemeChatTaskToFire task,
        IDbConnectionFactory connectionFactory
    )
    {
        _task = task;
        _connectionFactory = connectionFactory;
    }

    public async Task<ITaskToFire> Fire()
    {
        await DeleteEntry();
        return await _task.Fire();
    }

    public long SubjectId() => _task.SubjectId();

    public long ChatId() => _task.ChatId();

    public string Message() => _task.Message();

    public int ThemeChatId() => _task.ThemeChatId();

    public DateTime Created() => _task.Created();

    public DateTime Notified() => _task.Notified();

    public bool IsPeriodic() => _task.IsPeriodic();

    private async Task DeleteEntry()
    {
        var parameters = new { subjectId = SubjectId(), themeId = ThemeChatId() };
        CommandDefinition command = new(_sql, parameters);
        using IDbConnection connection = _connectionFactory.Create(
            SqliteConstants.NotificationsConnectionString
        );
        await connection.ExecuteAsync(command);
    }
}
