using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChatTheme;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;
using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterTheme;
using RocketTaskPlanner.Application.NotificationsContext.Visitor;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;

namespace RocketTaskPlanner.Application.Facades;

public sealed class UserThemeRegistrationFacade
{
    private readonly IExternalChatUseCasesVisitor _externalChatsVisitor;
    private readonly INotificationUseCaseVisitor _notificationChatsVisitor;
    private readonly IUnitOfWork _unitOfWork;

    public UserThemeRegistrationFacade(
        IExternalChatUseCasesVisitor externalChatsVisitor,
        INotificationUseCaseVisitor notificationChatsVisitor,
        IUnitOfWork unitOfWork
    )
    {
        _externalChatsVisitor = externalChatsVisitor;
        _notificationChatsVisitor = notificationChatsVisitor;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> RegisterUserTheme(
        long parentChatId,
        long themeChatId,
        long ownerId,
        string parentChatName
    )
    {
        using (_unitOfWork)
        {
            var addingChildChat = await AddChildChatForParent(
                parentChatId,
                themeChatId,
                ownerId,
                parentChatName
            );
            if (addingChildChat.IsFailure)
                return addingChildChat;

            var addingThemeChat = await AddThemeAsNotificationReceiver(parentChatId, themeChatId);
            if (addingThemeChat.IsFailure)
                return addingThemeChat;

            await _unitOfWork.Process();
            Result savingChanges = _unitOfWork.TryCommit();
            if (savingChanges.IsFailure)
                return savingChanges;
        }

        return Result.Success();
    }

    private async Task<Result> AddChildChatForParent(
        long parentChatId,
        long themeChatId,
        long ownerId,
        string chatName
    )
    {
        AddExternalChatThemeUseCase useCase = new(parentChatId, themeChatId, ownerId, chatName);
        return await _externalChatsVisitor.Visit(useCase);
    }

    private async Task<Result> AddThemeAsNotificationReceiver(long chatId, long themeId)
    {
        RegisterThemeUseCase useCase = new(chatId, themeId);
        return await _notificationChatsVisitor.Visit(useCase);
    }
}
