using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChatTheme;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;
using RocketTaskPlanner.Application.NotificationsContext.Features.RemoveTheme;
using RocketTaskPlanner.Application.NotificationsContext.Visitor;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;

namespace RocketTaskPlanner.Application.Facades;

public sealed class RemoveThemeChatFacade
{
    private readonly IExternalChatUseCasesVisitor _userChats;
    private readonly INotificationUseCaseVisitor _notificationChats;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveThemeChatFacade(
        IExternalChatUseCasesVisitor userChats,
        INotificationUseCaseVisitor notificationChats,
        IUnitOfWork unitOfWork
    )
    {
        _userChats = userChats;
        _notificationChats = notificationChats;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> RemoveThemeChat(long userId, long chatId, long themeId)
    {
        using (_unitOfWork)
        {
            Result removingUserChat = await RemoveUserChildChat(userId, chatId, themeId);
            if (removingUserChat.IsFailure)
                return removingUserChat;

            Result removingNotificationsTheme = await RemoveNotificationThemeChat(chatId, themeId);
            if (removingNotificationsTheme.IsFailure)
                return removingNotificationsTheme;

            await _unitOfWork.Process();
            Result commit = _unitOfWork.TryCommit();
            if (commit.IsFailure)
                return commit;
        }

        return Result.Success();
    }

    private async Task<Result> RemoveNotificationThemeChat(long chatId, long themeId)
    {
        var useCase = new RemoveThemeUseCase(chatId, themeId);
        return await _notificationChats.Visit(useCase);
    }

    private async Task<Result> RemoveUserChildChat(long userId, long chatId, long themeId)
    {
        var useCase = new RemoveExternalChatThemeUseCase(userId, chatId, themeId);
        return await _userChats.Visit(useCase);
    }
}
