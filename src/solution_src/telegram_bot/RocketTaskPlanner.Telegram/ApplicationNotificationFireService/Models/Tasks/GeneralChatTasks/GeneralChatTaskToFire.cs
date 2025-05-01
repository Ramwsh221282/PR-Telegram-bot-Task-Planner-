using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks.GeneralChatTasks;

public sealed class GeneralChatTaskToFire : IGeneralChatTaskToFire
{
    private readonly GeneralChatSubjectEntity _subject;

    public GeneralChatTaskToFire(GeneralChatSubjectEntity subject) => _subject = subject;

    public async Task<ITaskToFire> Fire() => await Task.FromResult(this);

    public long SubjectId() => _subject.GeneralChatSubjectId;

    public long ChatId() => _subject.GeneralChatId;

    public string Message() => _subject.SubjectMessage;

    public DateTime Created() => _subject.SubjectCreated;

    public DateTime Notified() => _subject.SubjectNotify;

    public bool IsPeriodic()
    {
        int periodInfo = _subject.SubjectPeriodic;
        return periodInfo != 0;
    }
}
