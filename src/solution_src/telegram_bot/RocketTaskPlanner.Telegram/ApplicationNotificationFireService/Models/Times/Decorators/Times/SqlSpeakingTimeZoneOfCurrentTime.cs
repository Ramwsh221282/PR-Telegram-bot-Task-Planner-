using System.Data;
using Dapper;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Entities;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Receivers;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Times.Decorators.Times;

/// <summary>
/// Декоратор временной зоны для общения с БД (инициализацией получателей)
/// </summary>
public sealed class SqlSpeakingTimeZoneOfCurrentTime : ITimeZoneOfCurrentTime
{
    private readonly IDbConnectionFactory _connectionFactory;
    private ITimeZoneOfCurrentTime _time;

    public SqlSpeakingTimeZoneOfCurrentTime(
        TimeZoneOfCurrentTime time,
        IDbConnectionFactory connectionFactory
    )
    {
        _time = time;
        _connectionFactory = connectionFactory;
    }

    public string ZoneName() => _time.ZoneName();

    public DateTime DateTime() => _time.DateTime();

    public string Id() => _time.Id();

    public async Task<GeneralChatReceiverOfCurrentTimeZone[]> Receivers()
    {
        GeneralChatReceiverOfCurrentTimeZone[] receivers = await _time.Receivers();
        if (receivers.Length > 0)
            return receivers;
        receivers = await QueryReceivers();
        _time = _time.WithReceivers(receivers);
        return await _time.Receivers();
    }

    public ITimeZoneOfCurrentTime WithReceivers(GeneralChatReceiverOfCurrentTimeZone[] receivers)
    {
        _time = _time.WithReceivers(receivers);
        return _time;
    }

    private async Task<GeneralChatReceiverOfCurrentTimeZone[]> QueryReceivers()
    {
        using IDbConnection connection = _connectionFactory.Create();
        NotificationReceiverEntity[] generalChats = await QueryGeneralChats(connection);
        GeneralChatReceiverOfCurrentTimeZone[] receivers = new GeneralChatReceiverOfCurrentTimeZone[
            generalChats.Length
        ];
        for (int index = 0; index < generalChats.Length; index++)
        {
            NotificationReceiverEntity target = generalChats[index];
            NotificationReceiverEntity withSubjects = await WithGeneralChatSubjects(target, connection);
            NotificationReceiverEntity withThemes = await WithThemes(withSubjects, connection);
            receivers[index] = new GeneralChatReceiverOfCurrentTimeZone(withThemes);
        }
        return receivers;
    }

    private async Task<NotificationReceiverEntity[]> QueryGeneralChats(IDbConnection connection)
    {
        const string sql = """
            SELECT 
            nr.receiver_id, 
            nr.receiver_name, 
            nr.receiver_zone_name
            FROM notification_receivers nr
            WHERE nr.receiver_zone_name = @zoneName
            """;
        var parameters = new { zoneName = _time.ZoneName() };
        CommandDefinition command = new(sql, parameters);
        IEnumerable<NotificationReceiverEntity> receivers =
            await connection.QueryAsync<NotificationReceiverEntity>(command);
        return receivers.ToArray();
    }

    private async Task<NotificationReceiverEntity> WithGeneralChatSubjects(
        NotificationReceiverEntity receiver,
        IDbConnection connection
    )
    {
        const string sql = """
            SELECT
            gcs.general_chat_subject_id,
            gcs.general_chat_id,
            gcs.subject_created,
            gcs.subject_message,
            gcs.subject_notify,
            gcs.subject_periodic
            FROM general_chat_subjects gcs
            WHERE gcs.general_chat_id = @generalChatId AND gcs.subject_notify <= @zoneDate
            """;
        var parameters = new { generalChatId = receiver.ReceiverId, zoneDate = _time.DateTime() };
        CommandDefinition command = new(sql, parameters);
        IEnumerable<GeneralChatSubjectEntity> subjects =
            await connection.QueryAsync<GeneralChatSubjectEntity>(command);
        foreach (GeneralChatSubjectEntity entry in subjects)
            receiver.TryAddGeneralChatSubject(entry);
        return receiver;
    }

    private async Task<NotificationReceiverEntity> WithThemes(
        NotificationReceiverEntity generalChat,
        IDbConnection connection
    )
    {
        const string sql = """
            SELECT
            rt.theme_id,
            rt.receiver_id
            FROM receiver_themes rt
            WHERE rt.receiver_id = @receiverId
            """;
        var parameters = new { receiverId = generalChat.ReceiverId };
        CommandDefinition command = new(sql, parameters);
        IEnumerable<ReceiverThemeEntity> themes = await connection.QueryAsync<ReceiverThemeEntity>(
            command
        );
        foreach (ReceiverThemeEntity entry in themes)
        {
            ReceiverThemeEntity withThemeSubjects = await WithThemeChatSubjects(entry, connection);
            generalChat.TryAddTheme(withThemeSubjects);
        }
        return generalChat;
    }

    private async Task<ReceiverThemeEntity> WithThemeChatSubjects(
        ReceiverThemeEntity entity,
        IDbConnection connection
    )
    {
        const string sql = """
            SELECT
            tcs.theme_chat_subject_id,
            tcs.subject_created,
            tcs.subject_message,
            tcs.subject_notify,
            tcs.subject_periodic,
            tcs.theme_id
            FROM theme_chat_subjects tcs
            LEFT JOIN receiver_themes th ON th.theme_id = tcs.theme_id
            WHERE tcs.theme_id = @themeId AND tcs.subject_notify <= @zoneDate AND th.receiver_id = @mainChatId
            """;
        var parameters = new { themeId = entity.ThemeId, zoneDate = _time.DateTime(), mainChatId = entity.ReceiverId };
        CommandDefinition command = new(sql, parameters);
        IEnumerable<ThemeChatSubjectEntity> subjects =
            await connection.QueryAsync<ThemeChatSubjectEntity>(command);
        foreach (ThemeChatSubjectEntity entry in subjects)
            entity.TryAddSubject(entry);
        return entity;
    }
}
