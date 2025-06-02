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
    private const string Context = "Транзакция удаления чата.";

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

    private readonly Serilog.ILogger _logger;

    public RemoveOwnerChatFacade(
        IExternalChatUseCasesVisitor userChats,
        INotificationUseCaseVisitor notificationChats,
        IUnitOfWork unitOfWork,
        Serilog.ILogger logger
    )
    {
        _userChats = userChats;
        _notificationChats = notificationChats;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> RemoveOwnerChat(long userId, long chatId, bool isLastChat)
    {
        await _unitOfWork.BeginTransaction();
        _logger.Information("{Context}. Начата.", Context);

        // удаление чата
        _logger.Information("{Context}. Удаление чата.", Context);
        var removingChat = await RemoveUserChat(userId, chatId);
        if (removingChat.IsFailure)
        {
            _logger.Error("{Context}. Ошибка: {error}", Context, removingChat.Error);
            return removingChat;
        }

        var saving = await _unitOfWork.SaveChangesAsync();
        if (saving.IsFailure)
        {
            await _unitOfWork.RollBackTransaction();
            _logger.Error("{Context}. Ошибка: {error}", Context, removingChat.Error);
            return removingChat;
        }

        // удаление чата для уведомлений
        _logger.Information("{Context}. Удаление чата для уведомлений.", Context);
        var removingNotificationChat = await RemoveNotificationChat(chatId);
        if (removingNotificationChat.IsFailure)
        {
            _logger.Error("{Context}. Ошибка: {Error}", Context, removingNotificationChat.Error);
            return removingNotificationChat;
        }

        saving = await _unitOfWork.SaveChangesAsync();
        if (saving.IsFailure)
        {
            await _unitOfWork.RollBackTransaction();
            _logger.Error("{Context}. Ошибка: {Error}", Context, removingNotificationChat.Error);
            return removingChat;
        }

        // если это последний чат пользователя - удаляется пользователь
        if (isLastChat)
        {
            _logger.Information("{Context}. Удаление пользователя потому что нет чатов.", Context);
            var removeUser = await RemoveUserIfHasNoChatsLeft(userId);
            if (removeUser.IsFailure)
            {
                _logger.Error("{Context}. Ошибка: {Error}", Context, removeUser.Error);
                return removeUser;
            }

            saving = await _unitOfWork.SaveChangesAsync();
            if (saving.IsFailure)
            {
                await _unitOfWork.RollBackTransaction();
                _logger.Error("{Context}. Ошибка: {Error}", Context, removeUser.Error);
                return removingChat;
            }
        }

        var committing = await _unitOfWork.CommitTransaction();
        if (committing.IsFailure)
        {
            _logger.Error("{Context}. Ошибка: {Error}", Context, committing.Error);
        }
        else
        {
            _logger.Information("{Context}. Выполнена.", Context);
        }

        return committing;
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
