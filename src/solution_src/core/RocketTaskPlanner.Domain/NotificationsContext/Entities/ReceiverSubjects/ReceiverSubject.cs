using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;

namespace RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;

/// <summary>
/// Уведомление
/// </summary>
public abstract class ReceiverSubject
{
    /// <summary>
    /// Id
    /// </summary>
    public ReceiverSubjectId Id { get; protected set; }

    /// <summary>
    /// Информация о времени
    /// </summary>
    public ReceiverSubjectTimeInfo TimeInfo { get; protected set; }

    /// <summary>
    /// Текст сообщения
    /// </summary>
    public ReceiverSubjectMessage Message { get; protected set; } = null!;

    /// <summary>
    /// Информация о периодичности
    /// </summary>
    public ReceiverSubjectPeriodInfo Period { get; protected set; }
}
