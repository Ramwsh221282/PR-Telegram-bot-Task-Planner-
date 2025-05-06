using RocketTaskPlanner.Domain.ExternalChatsManagementContext;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;

/// <summary>
/// Контракт взаимодействия с хранилищем внешних данных об участниках внешних чатов (чтение)
/// </summary>
public interface IExternalChatsReadableRepository
{
    /// <summary>
    /// Получение обладателя внешнего чата, со всеми его чатами и дочерними чатами.
    /// </summary>
    /// <param name="id">Id обладателя чата.</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Result Success, если был найден. Result Failure если не был найден</returns>
    Task<Result<ExternalChatOwner>> GetExternalChatOwnerById(
        long id,
        CancellationToken ct = default
    );

    /// <summary>
    /// Узнать есть ли какие-либо основные чаты у пользователя
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>True если есть хотя бы 1 чат. False если нет чатов.</returns>
    Task<bool> IsLastUserChat(long userId, CancellationToken ct = default);

    /// <summary>
    /// Содержится ли чат с ID в системе
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>True если есть. False если нет</returns>
    Task<bool> ContainsChat(long chatId, CancellationToken ct = default);

    /// <summary>
    /// Содержится ли дочерний чат с ID
    /// </summary>
    /// <param name="chatId">ID основного чата</param>
    /// <param name="childChatId">ID дочернего чата</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>True если есть. False если нет</returns>
    Task<bool> ContainsChildChat(long chatId, long childChatId, CancellationToken ct = default);

    /// <summary>
    /// Зарегистрирован ли пользователь в системе
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>True если зарегистрирован. False если не зарегистрирован</returns>
    Task<bool> HasUserRegistered(long userId, CancellationToken ct = default);

    /// <summary>
    /// Обладает ли пользователь чатом
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="chatId">ID чата</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>True если обладает. False если не обладает.</returns>
    Task<bool> UserOwnsChat(long userId, long chatId, CancellationToken ct = default);
}
