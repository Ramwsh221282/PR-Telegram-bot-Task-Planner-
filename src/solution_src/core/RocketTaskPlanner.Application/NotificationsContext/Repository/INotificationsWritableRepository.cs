using RocketTaskPlanner.Application.Shared.UnitOfWorks;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

namespace RocketTaskPlanner.Application.NotificationsContext.Repository;

/// <summary>
/// Контракт взаимодействия с БД в контексте чатов и уведомлений (только запись).
/// </summary>
public interface INotificationsWritableRepository : IRepository
{
    /// <summary>
    /// Добавить чат для уведомлений
    /// <param name="receiver">
    ///     <inheritdoc cref="NotificationReceiver"/>
    /// </param>
    /// <param name="unitOfWork">
    ///     <inheritdoc cref="IUnitOfWork"/>
    /// </param>
    /// <param name="ct">
    ///     Токен отмены
    /// </param>
    /// <returns>
    /// Result Success.
    /// </returns>
    /// </summary>
    Task<Result> Add(
        NotificationReceiver receiver,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    );

    /// <summary>
    /// Добавить тему чата
    /// <param name="theme">
    ///     <inheritdoc cref="ReceiverTheme"/>
    /// </param>
    /// <param name="unitOfWork">
    ///     <inheritdoc cref="IUnitOfWork"/>
    /// </param>
    /// <param name="ct">
    ///     Токен отмены
    /// </param>
    /// <returns>
    /// Result Success
    /// </returns>
    /// </summary>
    Result AddTheme(ReceiverTheme theme, IUnitOfWork unitOfWork, CancellationToken ct = default);

    /// <summary>
    /// Добавить уведомление для темы чата
    /// <param name="subject">
    ///     <inheritdoc cref="ThemeChatSubject"/>
    /// </param>
    /// <param name="ct">
    ///     Токен отмены
    /// </param>
    /// <returns>
    ///     Result Success
    /// </returns>
    /// </summary>
    Task<Result> AddSubject(ThemeChatSubject subject, CancellationToken ct = default);

    /// <summary>
    /// Добавить уведомление для чата
    /// <param name="subject">
    ///     <inheritdoc cref="GeneralChatReceiverSubject"/>
    /// </param>
    /// <param name="ct">
    ///     Токен отмены
    /// </param>
    /// <returns>
    ///     Result Success
    /// </returns>
    /// </summary>
    Task<Result> AddSubject(GeneralChatReceiverSubject subject, CancellationToken ct = default);

    /// <summary>
    /// Удалить основной чат
    /// <param name="id">ID чата</param>
    /// <param name="unitOfWork">
    ///     <inheritdoc cref="IUnitOfWork"/>
    /// </param>
    /// <param name="ct">
    ///     Токен отмены
    /// </param>
    /// <returns>
    /// Result Success или Failure (если не найден)
    /// </returns>
    /// </summary>
    Result Remove(long? id, IUnitOfWork unitOfWork, CancellationToken ct = default);

    /// <summary>
    /// Удалить тему чата
    /// <param name="theme">
    ///     <inheritdoc cref="ReceiverTheme"/>
    /// </param>
    /// <param name="unit">
    ///     <inheritdoc cref="IUnitOfWork"/>
    /// </param>
    /// <param name="ct">
    ///     Токен отмены
    /// </param>
    /// <returns>
    /// Result Success или Failure (если не найден)
    /// </returns>
    /// </summary>
    Result RemoveTheme(ReceiverTheme theme, IUnitOfWork unit, CancellationToken ct = default);

    /// <summary>
    /// Изменить временную зону основного чата
    /// <param name="id">
    ///     <inheritdoc cref="NotificationReceiverId"/>
    /// </param>
    /// <param name="timeZone">
    ///     <inheritdoc cref="NotificationReceiverTimeZone"/>
    /// </param>
    /// <param name="ct">
    ///     Токен отмены
    /// </param>
    /// <returns>
    /// Result Success или Failure (если не найден)
    /// </returns>
    /// </summary>
    Task<Result> ChangeTimeZone(
        NotificationReceiverId id,
        NotificationReceiverTimeZone timeZone,
        CancellationToken ct = default
    );

    /// <summary>
    /// Удалить уведомление основного чата
    /// <param name="subjectId">
    ///     ID уведомления
    /// </param>
    /// <param name="ct">
    ///     Токен отмены
    /// </param>
    /// <returns>
    ///  Result Success или Failure (если не найден)
    /// </returns>
    /// </summary>
    Task<Result> RemoveGeneralChatSubject(long subjectId, CancellationToken ct = default);

    /// <summary>
    /// Удалить уведомление темы чаты
    /// <param name="subjectId">
    ///     ID уведомления
    /// </param>
    /// <param name="ct">
    ///     Токен отмены
    /// </param>
    /// <returns>
    /// Result Success или Failure (если не найден)
    /// </returns>
    /// </summary>
    Task<Result> RemoveThemeChatSubject(long subjectId, CancellationToken ct = default);
}
