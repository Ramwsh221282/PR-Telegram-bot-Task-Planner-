using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Repositories;

public sealed class NotificationReceiverSqliteRepository(
    IDbConnectionFactory factory,
    Serilog.ILogger logger
) : INotificationReceiverRepository
{
    private readonly IDbConnectionFactory _factory = factory;
    private readonly Serilog.ILogger _logger = logger;

    private const string HasReceiverWithIdSql = """
        SELECT COUNT(*) FROM notification_receivers WHERE receiver_id = @receiver_id
        """;

    private const string AddSql = """
        INSERT INTO notification_receivers (receiver_id, receiver_name, receiver_zone_time_stamp, receiver_zone_name)
        VALUES (@receiver_id, @receiver_name, @receiver_zone_time_stamp, @receiver_zone_name)
        """;

    public async Task<Result> Add(NotificationReceiver receiver, CancellationToken ct = default)
    {
        using IDbConnection connection = CreateConnection();
        int count = await connection.ExecuteScalarAsync<int>(
            HasReceiverWithIdSql,
            new { receiver_id = receiver.Id.Id }
        );
        if (count != 0)
            return Result.Failure($"Чат с ID: {receiver.Id} уже подписан.");
        using IDbTransaction transaction = CreateTransaction(connection);
        try
        {
            var parameters = new
            {
                receiver_id = receiver.Id.Id,
                receiver_name = receiver.Name.Name,
                receiver_zone_time_stamp = receiver.TimeZone.TimeStamp,
                receiver_zone_name = receiver.TimeZone.ZoneName,
            };
            CommandDefinition command = new(AddSql, parameters, cancellationToken: ct);
            await connection.ExecuteAsync(command);
            transaction.Commit();
            return Result.Success();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.Fatal(
                "{Context}. {Operation}. Exception: {Ex}.",
                nameof(NotificationReceiverSqliteRepository),
                nameof(Add),
                ex.Message
            );
            return Result.Failure("Произошла ошибка при добавлении чата.");
        }
    }

    private const string AddThemeSql = """
        INSERT INTO receiver_themes (theme_id, receiver_id)
        VALUES (@theme_id, @receiver_id)
        """;

    public async Task<Result> AddTheme(ReceiverTheme theme, CancellationToken ct = default)
    {
        var parameters = new
        {
            theme_id = theme.Id.Id,
            receiver_id = theme.NotificationReceiverId.Id,
        };
        CommandDefinition command = new(AddThemeSql, parameters, cancellationToken: ct);
        using IDbConnection connection = CreateConnection();
        using IDbTransaction transaction = CreateTransaction(connection);
        try
        {
            await connection.ExecuteAsync(command);
            transaction.Commit();
            return Result.Success();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.Fatal(
                "{Context}. {Operation}. Exception: {Ex}.",
                nameof(NotificationReceiverSqliteRepository),
                nameof(AddTheme),
                ex.Message
            );
            return Result.Failure("Произошла ошибка при добавлении темы чата.");
        }
    }

    private const string AddThemeChatSubjectSql = """
        INSERT INTO theme_chat_subjects (theme_chat_subject_id, theme_id, subject_periodic, subject_created, subject_notify, subject_message)
        VALUES (@theme_chat_subject_id, @theme_id, @subject_periodic, @subject_created, @subject_notify, @subject_message)
        """;

    public async Task<Result> AddSubject(ThemeChatSubject subject, CancellationToken ct = default)
    {
        var parameters = new
        {
            theme_chat_subject_id = subject.Id.Id,
            theme_id = subject.ThemeId.Id,
            subject_periodic = subject.Period.IsPeriodic,
            subject_created = subject.TimeInfo.Created.Value,
            subject_notify = subject.TimeInfo.Notify.Value,
            subject_message = subject.Message.Message,
        };
        CommandDefinition command = new(AddThemeChatSubjectSql, parameters, cancellationToken: ct);
        using IDbConnection connection = CreateConnection();
        using IDbTransaction transaction = CreateTransaction(connection);
        try
        {
            await connection.ExecuteAsync(command);
            transaction.Commit();
            return Result.Success();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.Fatal(
                "{Context}. {Operation}. Exception: {Ex}.",
                nameof(NotificationReceiverSqliteRepository),
                nameof(AddTheme),
                ex.Message
            );
            return Result.Failure("Произошла ошибка при добавлении уведомления чата.");
        }
    }

    private const string AddGeneralChatSubject = """
        INSERT INTO general_chat_subjects (general_chat_subject_id, general_chat_id, subject_periodic, subject_created, subject_notify, subject_message)
        VALUES (@general_chat_subject_id, @general_chat_id, @subject_periodic, @subject_created, @subject_notify, @subject_message)
        """;

    public async Task<Result> AddSubject(
        GeneralChatReceiverSubject subject,
        CancellationToken ct = default
    )
    {
        var parameters = new
        {
            general_chat_subject_id = subject.Id.Id,
            general_chat_id = subject.GeneralChatId.Id,
            subject_periodic = subject.Period.IsPeriodic,
            subject_created = subject.TimeInfo.Created.Value,
            subject_notify = subject.TimeInfo.Notify.Value,
            subject_message = subject.Message.Message,
        };
        CommandDefinition command = new(AddGeneralChatSubject, parameters, cancellationToken: ct);
        using IDbConnection connection = CreateConnection();
        using IDbTransaction transaction = CreateTransaction(connection);
        try
        {
            await connection.ExecuteAsync(command);
            transaction.Commit();
            return Result.Success();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.Fatal(
                "{Context}. {Operation}. Exception: {Ex}.",
                nameof(NotificationReceiverSqliteRepository),
                nameof(AddTheme),
                ex.Message
            );
            return Result.Failure("Произошла ошибка при добавлении уведомления чата.");
        }
    }

    private const string GetSql = """
            SELECT
            nr.receiver_id,
            nr.receiver_name,
            nr.receiver_zone_time_stamp,
            nr.receiver_zone_name,
            rt.theme_id,
            gc.general_chat_subject_id,
            gc.general_chat_id,
            gc.subject_periodic,
            gc.subject_created,
            gc.subject_notify,
            gc.subject_message,
            rts.theme_chat_subject_id,
            rts.theme_id,
            rts.subject_periodic,
            rts.subject_created,
            rts.subject_notify,
            rts.subject_message
        FROM notification_receivers nr
        LEFT JOIN receiver_themes rt ON rt.receiver_id = nr.receiver_id
        LEFT JOIN general_chat_subjects gc ON gc.general_chat_id = nr.receiver_id
        LEFT JOIN theme_chat_subjects rts ON rts.theme_id = rt.theme_id
        WHERE nr.receiver_id = @id
        """;

    public async Task<Result<NotificationReceiver>> GetById(
        long? id,
        CancellationToken ct = default
    )
    {
        if (id == null)
            return Result.Failure<NotificationReceiver>("Чат не найден.");

        var parameters = new { id = id.Value };
        CommandDefinition command = new(GetSql, parameters, cancellationToken: ct);
        Dictionary<long, NotificationReceiverEntity> receiverDictionary = [];
        using IDbConnection connection = CreateConnection();
        await connection.QueryAsync<
            NotificationReceiverEntity,
            ReceiverThemeEntity,
            GeneralChatSubjectEntity,
            ThemeChatSubjectEntity,
            NotificationReceiverEntity
        >(
            command,
            (receiver, theme, chatSubject, themeSubject) =>
            {
                if (!receiverDictionary.TryGetValue(receiver.ReceiverId, out var receiverEntry))
                {
                    receiverEntry = receiver;
                    receiverEntry.ReceiverThemes = [];
                    receiverEntry.ReceiverSubjects = [];
                    receiverDictionary.Add(receiverEntry.ReceiverId, receiverEntry);
                }

                if (theme != null)
                {
                    theme.Subjects ??= [];
                    ReceiverThemeEntity? existingTheme =
                        receiverEntry.ReceiverThemes.FirstOrDefault(t =>
                            t.ThemeId == theme.ThemeId
                        );

                    if (existingTheme == null)
                    {
                        theme.ReceiverId = receiverEntry.ReceiverId;
                        receiverEntry.ReceiverThemes.Add(theme);
                        existingTheme = theme;
                    }

                    if (themeSubject != null)
                    {
                        themeSubject.ThemeId = existingTheme.ThemeId;
                        existingTheme.Subjects.Add(themeSubject);
                    }
                }

                if (chatSubject != null)
                {
                    chatSubject.GeneralChatId = receiverEntry.ReceiverId;
                    receiverEntry.ReceiverSubjects.Add(chatSubject);
                }

                return receiverEntry;
            },
            splitOn: "theme_id,general_chat_subject_id,theme_chat_subject_id"
        );

        if (receiverDictionary.Count == 0)
            return Result.Failure<NotificationReceiver>("Чат не найден.");

        return NotificationReceiverEntity.MapToNotificationReceiver(
            receiverDictionary.Values.First()
        );
    }

    private const string RemoveSql = """
        DELETE FROM notification_receivers WHERE receiver_id = @id;
        """;

    public async Task<Result> Remove(long? id, CancellationToken ct = default)
    {
        if (id == null)
            return Result.Failure("Чат не был найден");
        using IDbConnection connection = CreateConnection();
        using IDbTransaction transaction = CreateTransaction(connection);
        try
        {
            var parameters = new { id = id.Value };
            CommandDefinition command = new(RemoveSql, parameters, cancellationToken: ct);
            int removedCount = await connection.ExecuteAsync(command);
            if (removedCount == 0)
                return Result.Failure($"Чат с ID: {id.Value} не найден.");
            transaction.Commit();
            return Result.Success();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.Fatal(
                "{Context}. {Operation}. Exception: {Ex}.",
                nameof(NotificationReceiverSqliteRepository),
                nameof(Remove),
                ex.Message
            );
            return Result.Failure("Произошла ошибка удалении чата.");
        }
    }

    private IDbConnection CreateConnection() =>
        _factory.Create(SqliteConstants.NotificationsConnectionString);

    private static IDbTransaction CreateTransaction(IDbConnection connection) =>
        connection.BeginTransaction();
}
