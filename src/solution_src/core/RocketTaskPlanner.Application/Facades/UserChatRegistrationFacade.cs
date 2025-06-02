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
    private const string Context = "Транзакция добавления чата.";

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

    private readonly Serilog.ILogger _logger;

    public UserChatRegistrationFacade(
        IExternalChatUseCasesVisitor externalChatsVisitor,
        INotificationUseCaseVisitor notificationChatsVisitor,
        IUnitOfWork unitOfWork,
        Serilog.ILogger logger
    )
    {
        _unitOfWork = unitOfWork;
        _externalChatsVisitor = externalChatsVisitor;
        _notificationChatsVisitor = notificationChatsVisitor;
        _logger = logger;
    }

    public async Task<Result> AddUserExternalChat(
        long ownerId,
        long chatId,
        string chatName,
        string timeZone
    )
    {
        await _unitOfWork.BeginTransaction();
        _logger.Information("{Context}. Начата.", Context);

        // добавление чата пользователя
        _logger.Information("{Context}. Добавление чата пользователя.", Context);
        var addingChat = await AddUserExternalChat(ownerId, chatId, chatName);
        if (addingChat.IsFailure)
        {
            _logger.Error("{Context}. Ошибка: {Error}", Context, addingChat.Error);
            return addingChat;
        }

        var saving = await _unitOfWork.SaveChangesAsync();
        if (saving.IsFailure)
        {
            await _unitOfWork.RollBackTransaction();
            _logger.Error("{Context} Ошибка: {Error}", Context, addingChat.Error);
            return saving;
        }

        // добавление чата для уведомлений
        _logger.Information("{Context}. Добавление чата для уведомлений.", Context);
        var addingNotificationChat = await AddChatForNotifications(chatId, chatName, timeZone);
        if (addingNotificationChat.IsFailure)
        {
            _logger.Error("{Context}. Ошибка: {Error}", Context, addingNotificationChat.Error);
            return addingNotificationChat;
        }

        saving = await _unitOfWork.SaveChangesAsync();
        if (saving.IsFailure)
        {
            await _unitOfWork.RollBackTransaction();
            _logger.Error("{Context}. Ошибка: {Error}", Context, saving.Error);
            return saving;
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
