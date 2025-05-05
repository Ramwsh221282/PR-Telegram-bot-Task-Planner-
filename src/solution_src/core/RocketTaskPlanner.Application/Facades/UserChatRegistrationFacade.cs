using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChat;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;
using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterChat;
using RocketTaskPlanner.Application.NotificationsContext.Visitor;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;

namespace RocketTaskPlanner.Application.Facades;

public sealed class UserChatRegistrationFacade
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExternalChatUseCasesVisitor _externalChatsVisitor;
    private readonly INotificationUseCaseVisitor _notificationChatsVisitor;

    public UserChatRegistrationFacade(
        IExternalChatUseCasesVisitor externalChatsVisitor,
        INotificationUseCaseVisitor notificationChatsVisitor,
        IUnitOfWork unitOfWork
    )
    {
        _unitOfWork = unitOfWork;
        _externalChatsVisitor = externalChatsVisitor;
        _notificationChatsVisitor = notificationChatsVisitor;
    }

    public async Task<Result> AddUserExternalChat(
        long ownerId,
        long chatId,
        string chatName,
        string timeZone
    )
    {
        using (_unitOfWork)
        {
            var addingChat = await AddUserExternalChat(ownerId, chatId, chatName);
            if (addingChat.IsFailure)
                return addingChat;

            var addingNotificationChat = await AddChatForNotifications(chatId, chatName, timeZone);
            if (addingNotificationChat.IsFailure)
                return addingNotificationChat;

            await _unitOfWork.Process();
            var commit = _unitOfWork.TryCommit();
            if (commit.IsFailure)
                return commit;
        }

        return Result.Success();
    }

    private async Task<Result> AddUserExternalChat(long ownerId, long chatId, string chatName)
    {
        AddExternalChatUseCase useCase = new(ownerId, chatId, chatName);
        return await _externalChatsVisitor.Visit(useCase);
    }

    private async Task<Result> AddChatForNotifications(
        long chatId,
        string ownerName,
        string timeZone
    )
    {
        RegisterChatUseCase useCase = new(chatId, ownerName, timeZone);
        return await _notificationChatsVisitor.Visit(useCase);
    }
}
