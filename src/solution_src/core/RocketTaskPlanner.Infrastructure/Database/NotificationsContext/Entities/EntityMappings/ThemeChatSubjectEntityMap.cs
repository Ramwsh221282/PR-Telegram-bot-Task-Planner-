using Dapper.FluentMap.Mapping;

namespace RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Entities.EntityMappings;

/// <summary>
/// Маппинг уведомления темы чата в Dao модель темы чата при запросах через Dapper
/// </summary>
public sealed class ThemeChatSubjectEntityMap : EntityMap<ThemeChatSubjectEntity>
{
    public ThemeChatSubjectEntityMap()
    {
        Map(s => s.ThemeChatSubjectId).ToColumn("theme_chat_subject_id");
        Map(s => s.ThemeId).ToColumn("theme_id");
        Map(s => s.SubjectPeriodic).ToColumn("subject_periodic");
        Map(s => s.SubjectCreated).ToColumn("subject_created");
        Map(s => s.SubjectNotify).ToColumn("subject_notify");
        Map(s => s.SubjectMessage).ToColumn("subject_message");
    }
}
