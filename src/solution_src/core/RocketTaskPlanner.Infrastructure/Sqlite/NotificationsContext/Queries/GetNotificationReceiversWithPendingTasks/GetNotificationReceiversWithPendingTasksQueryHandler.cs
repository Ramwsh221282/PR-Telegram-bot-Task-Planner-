using System.Data;
using Dapper;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetNotificationReceiversWithPendingTasks;

public sealed class GetNotificationReceiversWithPendingTasksQueryHandler(
    IDbConnectionFactory factory
) : IQueryHandler<GetNotificationReceiversWithPendingTasksQuery, NotificationReceiverEntity[]>
{
    private readonly IDbConnectionFactory _factory = factory;

    private const string SQL = """
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
        WHERE nr.receiver_zone_name = @zone_name 
        """;

    public async Task<NotificationReceiverEntity[]> Handle(
        GetNotificationReceiversWithPendingTasksQuery query,
        CancellationToken ct = default
    )
    {
        Dictionary<long, NotificationReceiverEntity> receiverEntries = [];
        var parameters = new { zone_name = query.ZoneName, current_date = query.ZoneDateTime };
        CommandDefinition command = new(SQL, parameters, cancellationToken: ct);
        using IDbConnection connection = _factory.Create(
            SqliteConstants.NotificationsConnectionString
        );
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
                if (
                    !receiverEntries.TryGetValue(
                        receiver.ReceiverId,
                        out NotificationReceiverEntity currentEntry
                    )
                )
                {
                    currentEntry = receiver;
                    currentEntry.ReceiverId = receiver.ReceiverId;
                    currentEntry.ReceiverName = receiver.ReceiverName;
                    currentEntry.ReceiverSubjects = [];
                    currentEntry.ReceiverThemes = [];
                    currentEntry.ReceiverZoneName = receiver.ReceiverZoneName;
                    currentEntry.ReceiverZoneTimeStamp = receiver.ReceiverZoneTimeStamp;
                    receiverEntries.Add(currentEntry.ReceiverId, currentEntry);
                }

                if (
                    theme is not null
                    && !currentEntry.ReceiverThemes.Any(t => t.ThemeId == theme.ThemeId)
                )
                {
                    theme.ReceiverId = currentEntry.ReceiverId;

                    if (
                        themeSubject is not null
                        && !theme.Subjects.Any(s =>
                            s.ThemeChatSubjectId == themeSubject.ThemeChatSubjectId
                        )
                    )
                    {
                        themeSubject.ThemeId = theme.ThemeId;
                        theme.Subjects.Add(themeSubject);
                    }

                    currentEntry.ReceiverThemes.Add(theme);
                }

                if (
                    chatSubject is not null
                    && !currentEntry.ReceiverSubjects.Any(s =>
                        s.GeneralChatSubjectId == chatSubject.GeneralChatSubjectId
                    )
                )
                {
                    chatSubject.GeneralChatId = currentEntry.ReceiverId;
                    currentEntry.ReceiverSubjects.Add(chatSubject);
                }

                return currentEntry;
            },
            splitOn: "theme_id,general_chat_subject_id,theme_chat_subject_id"
        );

        NotificationReceiverEntity[] receivers = [.. receiverEntries.Values.Select(v => v)];
        return receivers;
    }
}
