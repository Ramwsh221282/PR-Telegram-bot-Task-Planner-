using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Database;
using RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks.ThemeChatTasks;

/// <summary>
/// Декоратор сообщения темы чата для работы с базой данных
/// </summary>
public sealed class ThemeChatTaskToFireSqlSpeaking : IThemeChatTaskToFire
{
    private const string _deleteSql = """
        DELETE FROM theme_chat_subjects
        WHERE
        theme_chat_subject_id = @subjectId
        AND
        theme_id = @themeId
        """;
    private const string _updateSql = """
        UPDATE theme_chat_subjects
        SET subject_created = @created, subject_notify = @notify
        WHERE theme_chat_subject_id = @subjectId AND theme_id = @themeId
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
        // при вызове уведомления его нужно либо удалить, либо продлить (если периодическое)
        bool isPeriodic = IsPeriodic();
        Task operation = isPeriodic switch
        {
            true => UpdateRecord(),
            false => DeleteEntry(),
        };
        await operation;
        return await _task.Fire();
    }

    public long SubjectId() => _task.SubjectId();

    public long ChatId() => _task.ChatId();

    public string Message() => _task.Message();

    public int ThemeChatId() => _task.ThemeChatId();

    public DateTime Created() => _task.Created();

    public DateTime Notified() => _task.Notified();

    public bool IsPeriodic() => _task.IsPeriodic();

    /// <summary>
    /// Удаление сообщения из БД.
    /// </summary>
    private async Task DeleteEntry()
    {
        var parameters = new { subjectId = SubjectId(), themeId = ThemeChatId() };
        CommandDefinition command = new(_deleteSql, parameters);
        using IDbConnection connection = _connectionFactory.Create();
        await connection.ExecuteAsync(command);
    }

    private async Task UpdateRecord()
    {
        long subjectId = SubjectId();
        long themeId = ThemeChatId();
        DateTime newDateCreated = Notified();
        DateTime newDateNotified = await NextNotifyDate();
        var parameters = new
        {
            subjectId,
            themeId,
            created = newDateCreated,
            notify = newDateNotified,
        };
        using IDbConnection connection = _connectionFactory.Create();
        await connection.ExecuteAsync(_updateSql, parameters);
    }

    /// <summary>
    /// Расчет следующего времени уведомления, через повторное распознавание времени на основе текста сообщения.
    /// </summary>
    /// <returns>Дата следующего уведомления</returns>
    private async Task<DateTime> NextNotifyDate()
    {
        DateTime notified = Notified();
        string message = Message();
        TimeRecognitionFacade timeRecognition = new();
        Result<TimeRecognitionResult> recognizedTimeResult = await timeRecognition.RecognizeTime(
            message
        );

        TimeRecognitionResult recognizedTime = recognizedTimeResult.Value;
        TimeCalculationService timeCalculation = new();
        TimeCalculationItem current = new(notified, true);
        TimeCalculationItem updatedTime = timeCalculation.AddOffset(current, recognizedTime);
        return updatedTime.CalculationDateTime;
    }
}
