using Dapper.FluentMap.Mapping;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities.EntityMappings;

public sealed class GeneralChatSubjectEntityMap : EntityMap<GeneralChatSubjectEntity>
{
    public GeneralChatSubjectEntityMap()
    {
        Map(s => s.GeneralChatSubjectId).ToColumn("general_chat_subject_id");
        Map(s => s.GeneralChatId).ToColumn("general_chat_id");
        Map(s => s.SubjectPeriodic).ToColumn("subject_periodic");
        Map(s => s.SubjectCreated).ToColumn("subject_created");
        Map(s => s.SubjectNotify).ToColumn("subject_notify");
        Map(s => s.SubjectMessage).ToColumn("subject_message");
    }
}
