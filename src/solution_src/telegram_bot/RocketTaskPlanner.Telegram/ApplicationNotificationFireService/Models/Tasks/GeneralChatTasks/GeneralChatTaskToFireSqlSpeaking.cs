using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite;
using RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks.GeneralChatTasks;

/// <summary>
/// Декоратор для работы с базой данных
/// </summary>
public sealed class GeneralChatTaskToFireSqlSpeaking : IGeneralChatTaskToFire
{
    private const string _deleteSql = """
        DELETE FROM 
        general_chat_subjects
        WHERE general_chat_subject_id = @subjectId AND general_chat_id = @chatId
        """;
    private const string _updateSql = """
        UPDATE general_chat_subjects
        SET subject_created = @dateCreated, subject_notify = @subjectNotify
        WHERE general_chat_subject_id = @subjectId AND general_chat_id = @chatId
        """;
    private readonly IGeneralChatTaskToFire _task;
    private readonly IDbConnectionFactory _connectionFactory;

    public GeneralChatTaskToFireSqlSpeaking(
        IGeneralChatTaskToFire task,
        IDbConnectionFactory connectionFactory
    )
    {
        _task = task;
        _connectionFactory = connectionFactory;
    }

    public async Task<ITaskToFire> Fire()
    {
        bool isPeriodic = IsPeriodic();

        // удаление сообщения, либо его продление, если периодичное.
        Task sqlOperation = isPeriodic switch
        {
            true => UpdateRecord(),
            false => DeleteRecord(),
        };
        await sqlOperation;
        return await _task.Fire();
    }

    public long SubjectId() => _task.SubjectId();

    public long ChatId() => _task.ChatId();

    public string Message() => _task.Message();

    public DateTime Created() => _task.Created();

    public DateTime Notified() => _task.Notified();

    public bool IsPeriodic() => _task.IsPeriodic();

    private async Task UpdateRecord()
    {
        long subjectId = SubjectId();
        long chatId = ChatId();
        DateTime newDateCreated = Notified();
        DateTime newDateNotified = await NextNotifyDate();
        var parameters = new
        {
            subjectId,
            chatId,
            dateCreated = newDateCreated,
            subjectNotify = newDateNotified,
        };
        using IDbConnection connection = _connectionFactory.Create(
            SqliteConstants.NotificationsConnectionString
        );
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
        TimeCalculationItem current = new(0, notified, true);
        TimeCalculationItem updatedTime = timeCalculation.AddOffset(current, recognizedTime);
        return updatedTime.CalculationDateTime;
    }

    private async Task DeleteRecord()
    {
        var parameters = new { subjectId = SubjectId(), chatId = ChatId() };
        CommandDefinition command = new(_deleteSql, parameters);
        using IDbConnection connection = _connectionFactory.Create(
            SqliteConstants.NotificationsConnectionString
        );
        await connection.ExecuteAsync(command);
    }
}
