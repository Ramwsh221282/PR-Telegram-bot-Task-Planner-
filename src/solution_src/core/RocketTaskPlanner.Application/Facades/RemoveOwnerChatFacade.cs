using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChat;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChatOwner;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;
using RocketTaskPlanner.Application.NotificationsContext.Features.RemoveChat;
using RocketTaskPlanner.Application.NotificationsContext.Visitor;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;

namespace RocketTaskPlanner.Application.Facades;

/// <summary>
/// Фасадный класс для удаления пользовательского чата
/// </summary>
public sealed class RemoveOwnerChatFacade
{
    /// <summary>
    /// <inheritdoc cref="IUnitOfWork"/>
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// <inheritdoc cref="IExternalChatUseCasesVisitor"/>
    /// </summary>
    private readonly IExternalChatUseCasesVisitor _userChats;

    /// <summary>
    /// <inheritdoc cref="INotificationUseCaseVisitor"/>
    /// </summary>
    private readonly INotificationUseCaseVisitor _notificationChats;

    public RemoveOwnerChatFacade(
        IExternalChatUseCasesVisitor userChats,
        INotificationUseCaseVisitor notificationChats,
        IUnitOfWork unitOfWork
    )
    {
        _userChats = userChats;
        _notificationChats = notificationChats;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> RemoveOwnerChat(long userId, long chatId, bool isLastChat)
    {
        using (_unitOfWork)
        {
            var removingChat = await RemoveUserChat(userId, chatId);
            if (removingChat.IsFailure)
                return removingChat;

            var removingNotificationChat = await RemoveNotificationChat(chatId);
            if (removingNotificationChat.IsFailure)
                return removingNotificationChat;

            if (isLastChat)
            {
                var removeUser = await RemoveUserIfHasNoChatsLeft(userId);
                if (removeUser.IsFailure)
                    return removeUser;
            }

            await _unitOfWork.Process();
            Result commit = _unitOfWork.TryCommit();
            if (commit.IsFailure)
                return commit;
        }

        return Result.Success();
    }

    // удалить пользователя, если у него был последний чат
    private async Task<Result> RemoveUserIfHasNoChatsLeft(long userId)
    {
        var useCase = new RemoveExternalChatOwnerUseCase(userId);
        return await _userChats.Visit(useCase);
    }

    // удалить пользовательский чат
    private async Task<Result> RemoveUserChat(long userId, long chatId)
    {
        var useCase = new RemoveExternalChatUseCase(userId, chatId);
        return await _userChats.Visit(useCase);
    }

    // удалить пользовательский чат для уведомлений
    private async Task<Result> RemoveNotificationChat(long chatId)
    {
        var useCase = new RemoveChatUseCase(chatId);
        return await _notificationChats.Visit(useCase);
    }
}
