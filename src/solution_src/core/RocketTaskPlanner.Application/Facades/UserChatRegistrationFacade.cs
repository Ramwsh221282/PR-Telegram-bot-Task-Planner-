using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChat;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;
using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterChat;
using RocketTaskPlanner.Application.NotificationsContext.Visitor;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;

namespace RocketTaskPlanner.Application.Facades;

/// <summary>
/// Фасадный класс для добавления чата пользователя
/// </summary>
public sealed class UserChatRegistrationFacade
{
    /// <summary>
    /// <inheritdoc cref="IUnitOfWork"/>
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// <inheritdoc cref="IExternalChatUseCasesVisitor"/>
    /// </summary>
    private readonly IExternalChatUseCasesVisitor _externalChatsVisitor;

    /// <summary>
    /// <inheritdoc cref="INotificationUseCaseVisitor"/>
    /// </summary>
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
            // добавление чата пользователя
            var addingChat = await AddUserExternalChat(ownerId, chatId, chatName);
            if (addingChat.IsFailure)
                return addingChat;

            // добавление чата для уведомлений
            var addingNotificationChat = await AddChatForNotifications(chatId, chatName, timeZone);
            if (addingNotificationChat.IsFailure)
                return addingNotificationChat;

            // выполнение команд
            await _unitOfWork.Process();

            // попытка сохранить результаты выполнения команд
            var commit = _unitOfWork.TryCommit();

            if (commit.IsFailure)
                return commit;
        }

        return Result.Success();
    }

    // добавить чат пользователю
    private async Task<Result> AddUserExternalChat(long ownerId, long chatId, string chatName)
    {
        AddExternalChatUseCase useCase = new(ownerId, chatId, chatName);
        return await _externalChatsVisitor.Visit(useCase);
    }

    // добавить чат для уведомлений
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
