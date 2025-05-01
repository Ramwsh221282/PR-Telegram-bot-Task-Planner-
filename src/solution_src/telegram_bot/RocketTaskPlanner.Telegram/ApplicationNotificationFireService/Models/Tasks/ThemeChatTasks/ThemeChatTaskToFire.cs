using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks.ThemeChatTasks;

public sealed class ThemeChatTaskToFire : IThemeChatTaskToFire
{
    private readonly ThemeChatSubjectEntity _subject;
    private readonly long _chatId;

    public ThemeChatTaskToFire(ThemeChatSubjectEntity subject, long chatId)
    {
        _subject = subject;
        _chatId = chatId;
    }

    public async Task<ITaskToFire> Fire() => await Task.FromResult(this);

    public long SubjectId() => _subject.ThemeChatSubjectId;

    public long ChatId() => _chatId;

    public string Message() => _subject.SubjectMessage;

    public int ThemeChatId() => (int)_subject.ThemeId;

    public DateTime Created() => _subject.SubjectCreated;

    public DateTime Notified() => _subject.SubjectNotify;

    public bool IsPeriodic()
    {
        int periodValue = _subject.SubjectPeriodic;
        return periodValue != 0;
    }
}
