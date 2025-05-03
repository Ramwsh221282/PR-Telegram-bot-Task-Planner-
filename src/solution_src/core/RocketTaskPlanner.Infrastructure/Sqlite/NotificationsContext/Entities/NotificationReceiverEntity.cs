using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities;

/// <summary>
/// Dao модель основного чата
/// </summary>
public sealed class NotificationReceiverEntity
{
    public long ReceiverId { get; set; }
    public string ReceiverName { get; set; } = string.Empty;
    public long ReceiverZoneTimeStamp { get; set; }
    public string ReceiverZoneName { get; set; } = string.Empty;

    public List<ReceiverThemeEntity> ReceiverThemes { get; set; } = [];
    public List<GeneralChatSubjectEntity> ReceiverSubjects { get; set; } = [];

    public NotificationReceiverEntity() { } //

    public NotificationReceiverEntity(NotificationReceiverEntity entity)
    {
        ReceiverId = entity.ReceiverId;
        ReceiverName = entity.ReceiverName;
        ReceiverZoneTimeStamp = entity.ReceiverZoneTimeStamp;
        ReceiverZoneName = entity.ReceiverZoneName;
        ReceiverThemes = entity.ReceiverThemes;
        ReceiverSubjects = entity.ReceiverSubjects;
    }

    public void TryAddGeneralChatSubject(GeneralChatSubjectEntity subject)
    {
        if (
            subject.GeneralChatId == ReceiverId
            && ReceiverSubjects.All(s => s.GeneralChatSubjectId != subject.GeneralChatSubjectId)
        )
            ReceiverSubjects.Add(subject);
    }

    public void TryAddTheme(ReceiverThemeEntity entity)
    {
        if (
            entity.ReceiverId != ReceiverId
            || ReceiverThemes.Any(s => s.ReceiverId == entity.ReceiverId)
        )
            return;
        entity.ReceiverId = ReceiverId;
        ReceiverThemes.Add(entity);
    }

    public NotificationReceiver ToNotificationReceiver()
    {
        NotificationReceiverId id = NotificationReceiverId.Create(ReceiverId).Value;
        NotificationReceiverName name = NotificationReceiverName.Create(ReceiverName).Value;
        NotificationReceiverTimeZone time = NotificationReceiverTimeZone
            .Create(ReceiverZoneName, ReceiverZoneTimeStamp)
            .Value;
        NotificationReceiver receiver = new NotificationReceiver()
        {
            Id = id,
            Name = name,
            TimeZone = time,
        };
        return receiver;
    }

    public static NotificationReceiver MapToNotificationReceiver(NotificationReceiverEntity entity)
    {
        NotificationReceiver receiver = entity.ToNotificationReceiver();
        GeneralChatReceiverSubject[] generalChatSubjects =
        [
            .. entity.ReceiverSubjects.Select(s => s.ToSubject(receiver)),
        ];

        foreach (ReceiverThemeEntity themeEntity in entity.ReceiverThemes)
        {
            ReceiverTheme theme = themeEntity.ToTheme(receiver);
            ThemeChatSubject[] themeSubjects =
            [
                .. themeEntity.Subjects.Select(s => s.ToSubject(theme)),
            ];
        }

        return receiver;
    }
}
