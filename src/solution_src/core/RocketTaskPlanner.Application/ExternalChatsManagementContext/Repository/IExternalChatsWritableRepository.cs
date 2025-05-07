using RocketTaskPlanner.Application.Shared.UnitOfWorks;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.Entities;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;

/// <summary>
/// Контракт взаимодействия с хранилищем внешних данных об участниках внешних чатов (запись)
/// </summary>
public interface IExternalChatsWritableRepository : IRepository
{
    /// <summary>
    /// Добавить обладателя чата
    /// <param name="externalChatOwner">ID обладателя чата</param>
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
    Result<ExternalChatOwner> AddChatOwner(
        ExternalChatOwner externalChatOwner,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    );

    /// <summary>
    /// Удалить обладателя чата
    /// <param name="externalChatOwner">
    ///     <inheritdoc cref="ExternalChatOwner"/>
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
    Result<ExternalChatOwner> RemoveChatOwner(
        ExternalChatOwner externalChatOwner,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    );

    /// <summary>
    /// Удалить чат у обладателя
    /// <param name="externalChat">
    ///     <inheritdoc cref="ExternalChat"/>
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
    Result<ExternalChat> RemoveChat(
        ExternalChat externalChat,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    );

    /// <summary>
    /// Добавить чат обладателю
    /// <param name="externalChat">
    ///     <inheritdoc cref="ExternalChat"/>
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
    Result<ExternalChat> AddChat(
        ExternalChat externalChat,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    );

    /// <summary>
    /// Добавить дочерний чат
    /// <param name="externalChat">
    ///     <inheritdoc cref="ExternalChat"/>
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
    Result<ExternalChat> AddThemeChat(
        ExternalChat externalChat,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    );

    /// <summary>
    /// Удалить дочерний чат
    /// <param name="externalChat">
    ///     <inheritdoc cref="ExternalChat"/>
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
    Result<ExternalChat> RemoveThemeChat(
        ExternalChat externalChat,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    );
}
