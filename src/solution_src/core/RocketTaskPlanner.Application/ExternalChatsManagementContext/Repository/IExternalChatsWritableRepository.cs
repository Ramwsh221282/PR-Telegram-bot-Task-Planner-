using RocketTaskPlanner.Application.Shared.UnitOfWorks;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.Entities;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;

/// <summary>
/// Контракт взаимодействия с хранилищем внешних данных об участниках внешних чатов (запись)
/// </summary>
public interface IExternalChatsWritableRepository : IRepository
{
    Result<ExternalChatOwner> AddChatOwner(
        ExternalChatOwner externalChatOwner,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    );

    Result<ExternalChatOwner> RemoveChatOwner(
        ExternalChatOwner externalChatOwner,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    );

    Result<ExternalChat> RemoveChat(
        ExternalChat externalChat,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    );

    Result<ExternalChat> AddChat(
        ExternalChat externalChat,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    );

    Result<ExternalChat> AddThemeChat(
        ExternalChat externalChat,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    );

    Result<ExternalChat> RemoveThemeChat(
        ExternalChat externalChat,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    );
}
