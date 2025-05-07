using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Repositories;

/// <summary>
/// Абстракция для работы с базой данных <inheritdoc cref="NotificationReceiver"/>
/// Выполняет только операции чтения.
/// </summary>
public sealed class NotificationsReadableRepository : INotificationsReadableRepository
{
    /// <summary>
    ///     <inheritdoc cref="IDbConnectionFactory"/>
    /// </summary>
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public NotificationsReadableRepository(IDbConnectionFactory connectionFactory)
    {
        _dbConnectionFactory = connectionFactory;
    }

    /// <summary>
    /// Получить NotificationReceiver с его чатами, темами, и уведомления (тем и чатов)
    /// </summary>
    /// <param name="id">ID чата</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns><inheritdoc cref="NotificationReceiver"/></returns>
    public async Task<Result<NotificationReceiver>> GetById(
        long? id,
        CancellationToken ct = default
    )
    {
        const string sql = """
                SELECT
                nr.receiver_id,
                nr.receiver_name,
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

        if (id == null)
            return Result.Failure<NotificationReceiver>("Чат не найден.");

        var parameters = new { id = id.Value };
        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
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

    /// <summary>
    /// <inheritdoc cref="IRepository.CreateConnection"/>
    /// </summary>
    /// <returns><inheritdoc cref="IDbConnection"/></returns>
    public IDbConnection CreateConnection() =>
        _dbConnectionFactory.Create(SqliteConstants.NotificationsConnectionString);

    /// <summary>
    /// Получить название чата по ID
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns><inheritdoc cref="NotificationReceiverName"/></returns>
    public async Task<Result<NotificationReceiverName>> GetNameById(
        long? id,
        CancellationToken ct = default
    )
    {
        const string sql = """
            SELECT receiver_name FROM notification_receivers WHERE receiver_id = @id
            """;

        var parameters = new { id };
        var command = new CommandDefinition(sql, parameters, cancellationToken: ct);
        using var connection = CreateConnection();
        var name = await connection.QueryFirstOrDefaultAsync<string>(command);

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<NotificationReceiverName>($"Не найден чат с ID: {id}");

        return NotificationReceiverName.Create(name).Value;
    }
}
