using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

namespace RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;

public sealed class ReceiverTheme
{
    private readonly List<ThemeChatSubject> _subjects = [];
    public ReceiverThemeId Id { get; } = default!;
    public NotificationReceiver NotificationReceiver { get; } = null!;
    public NotificationReceiverId NotificationReceiverId { get; } = default!;
    public IReadOnlyList<ThemeChatSubject> Subjects => _subjects;

    private ReceiverTheme() { } // ef core

    internal ReceiverTheme(ReceiverThemeId id, NotificationReceiver receiver)
    {
        Id = id;
        NotificationReceiver = receiver;
        NotificationReceiverId = receiver.Id;
    }

    public ThemeChatSubject AddSubject(
        ReceiverSubjectId id,
        ReceiverSubjectTimeInfo time,
        ReceiverSubjectPeriodInfo period,
        ReceiverSubjectMessage message
    )
    {
        ThemeChatSubject subject = new(id, time, message, period, this);
        _subjects.Add(subject);
        return subject;
    }

    public ReceiverSubject[] GetSubjects(Func<ReceiverSubject, bool> predicate) =>
        [.. _subjects.Where(predicate)];
}
