using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

namespace RocketTaskPlanner.Application.NotificationsContext.Repository;

/// <summary>
/// Контракт взаимодействия с БД для работы с уведомлениями и чатами уведомлений. (только чтение).
/// </summary>
public interface INotificationsReadableRepository
{
    /// <summary>
    /// Получить <inheritdoc cref="NotificationReceiver"/> по ID
    /// <param name="id">ID чата</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Result Success если найден. Failure если не найден</returns>
    /// </summary>
    Task<Result<NotificationReceiver>> GetById(long? id, CancellationToken ct = default);

    /// <summary>
    /// Получить имя <inheritdoc cref="NotificationReceiver"/> по ID
    /// <param name="id">ID чата</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Result Success если найден. Failure если не найден</returns>
    /// </summary>
    Task<Result<NotificationReceiverName>> GetNameById(long? id, CancellationToken ct = default);
}
