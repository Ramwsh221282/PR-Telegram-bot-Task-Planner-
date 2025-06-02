using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChatTheme;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;
using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterTheme;
using RocketTaskPlanner.Application.NotificationsContext.Visitor;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;

namespace RocketTaskPlanner.Application.Facades;

/// <summary>
/// Фасадный класс для добавления темы чата
/// </summary>
public sealed class UserThemeRegistrationFacade
{
    private const string Context = "Транзакция добавления темы.";

    /// <summary>
    /// <inheritdoc cref="IExternalChatUseCasesVisitor"/>
    /// </summary>
    private readonly IExternalChatUseCasesVisitor _externalChatsVisitor;

    /// <summary>
    /// <inheritdoc cref="INotificationUseCaseVisitor"/>
    /// </summary>
    private readonly INotificationUseCaseVisitor _notificationChatsVisitor;

    /// <summary>
    /// <inheritdoc cref="IUnitOfWork"/>
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;

    private readonly Serilog.ILogger _logger;

    public UserThemeRegistrationFacade(
        IExternalChatUseCasesVisitor externalChatsVisitor,
        INotificationUseCaseVisitor notificationChatsVisitor,
        IUnitOfWork unitOfWork,
        Serilog.ILogger logger
    )
    {
        _logger = logger;
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
        await _unitOfWork.BeginTransaction();
        _logger.Information("{Context}. Начато.", Context);

        // добавить дочерний чат пользователю
        _logger.Information("{Context}. Добавить дочерний чат пользователю.", Context);
        var addingChildChat = await AddChildChatForParent(
            parentChatId,
            themeChatId,
            ownerId,
            parentChatName
        );
        if (addingChildChat.IsFailure)
        {
            _logger.Error("{Context}. Ошибка: {Error}", Context, addingChildChat.Error);
            return addingChildChat;
        }

        var saving = await _unitOfWork.SaveChangesAsync();
        if (saving.IsFailure)
        {
            await _unitOfWork.RollBackTransaction();
            _logger.Error("{Context}. Ошибка. Error: {Error}", Context, saving.Error);
            return saving;
        }

        // добавление темы для уведомлений.
        _logger.Information("{Context}. Добавление темы для уведомлений.", Context);
        var addingThemeChat = await AddThemeAsNotificationReceiver(parentChatId, themeChatId);
        if (addingThemeChat.IsFailure)
        {
            _logger.Error("{Context}. Ошибка: {Error}", Context, addingThemeChat.Error);
            return addingThemeChat;
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
            _logger.Information("{Context}. Выполнено.", Context);
        }

        return committing;
    }

    // добавить дочерний чат
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

    // добавить тему чата для уведомлений
    private async Task<Result> AddThemeAsNotificationReceiver(long chatId, long themeId)
    {
        RegisterThemeUseCase useCase = new(chatId, themeId);
        return await _notificationChatsVisitor.Visit(useCase);
    }
}
