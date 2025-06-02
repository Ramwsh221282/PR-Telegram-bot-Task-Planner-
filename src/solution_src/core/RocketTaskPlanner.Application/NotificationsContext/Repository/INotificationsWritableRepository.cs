using RocketTaskPlanner.Domain.NotificationsContext;

namespace RocketTaskPlanner.Application.NotificationsContext.Repository;

/// <summary>
/// Контракт взаимодействия с БД в контексте чатов и уведомлений (только запись).
/// </summary>
public interface INotificationsWritableRepository : IDisposable
{
    /// <summary>
    /// Добавить чат для уведомлений
    /// <param name="receiver">
    ///     <inheritdoc cref="NotificationReceiver"/>
    /// </param>
    /// <param name="ct">
    ///     Токен отмены
    /// </param>
    /// <returns>
    /// Result Success.
    /// </returns>
    /// </summary>
    Task<Result<NotificationReceiver>> Add(
        NotificationReceiver receiver,
        CancellationToken ct = default);

    /// <summary>
    /// Удалить основной чат
    /// <returns>
    /// Result Success или Failure (если не найден)
    /// </returns>
    /// </summary>
    void Remove(NotificationReceiver receiver);
    
    Task<Result<NotificationReceiver>> GetById(long receiverId, CancellationToken ct = default);
    
    Task<Result>RemoveGeneralChatSubject(long subjectId, CancellationToken ct = default);

    Task<Result> RemoveThemeChatSubject(long subjectId, CancellationToken ct = default);
}
