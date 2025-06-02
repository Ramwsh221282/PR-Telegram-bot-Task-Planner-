using RocketTaskPlanner.Application.Shared.UnitOfWorks;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.Entities;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;

/// <summary>
/// Контракт взаимодействия с хранилищем внешних данных об участниках внешних чатов (запись)
/// </summary>
public interface IExternalChatsWritableRepository : IDisposable
{
    /// <summary>
    /// Добавить обладателя чата
    /// <param name="externalChatOwner">ID обладателя чата</param>
    /// <param name="ct">
    ///     Токен отмены
    /// </param>
    /// <returns>
    /// Result Success
    /// </returns>
    /// </summary>
    Task<Result<ExternalChatOwner>> AddChatOwner(
        ExternalChatOwner externalChatOwner,
        CancellationToken ct = default);

    /// <summary>
    /// Удалить обладателя чата
    /// <param name="externalChatOwner">
    ///     <inheritdoc cref="ExternalChatOwner"/>
    /// </param>
    /// <returns>
    /// Result Success
    /// </returns>
    /// </summary>
    void RemoveChatOwner(ExternalChatOwner externalChatOwner);

    Task<Result<ExternalChatOwner>> GetById(long ownerId, CancellationToken ct = default);
}
