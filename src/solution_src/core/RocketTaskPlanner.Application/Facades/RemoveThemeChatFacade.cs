using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChatTheme;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;
using RocketTaskPlanner.Application.NotificationsContext.Features.RemoveTheme;
using RocketTaskPlanner.Application.NotificationsContext.Visitor;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;

namespace RocketTaskPlanner.Application.Facades;

/// <summary>
/// Фасадный класс удаления темы чата
/// </summary>
public sealed class RemoveThemeChatFacade
{
    private const string Context = "Транзакция удаления темы.";

    /// <summary>
    /// <inheritdoc cref="IExternalChatUseCasesVisitor"/>
    /// </summary>
    private readonly IExternalChatUseCasesVisitor _userChats;

    /// <summary>
    /// <inheritdoc cref="INotificationUseCaseVisitor"/>
    /// </summary>
    private readonly INotificationUseCaseVisitor _notificationChats;

    /// <summary>
    /// <inheritdoc cref="IUnitOfWork"/>
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;

    private readonly Serilog.ILogger _logger;

    public RemoveThemeChatFacade(
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

    public async Task<Result> RemoveThemeChat(long userId, long chatId, long themeId)
    {
        await _unitOfWork.BeginTransaction();
        _logger.Information("{Context}. Начато.", Context);

        // удаление чата у пользователя
        _logger.Information("{Context}. Удаление чата пользователя.", Context);
        var removingUserChat = await RemoveUserChildChat(userId, chatId, themeId);
        if (removingUserChat.IsFailure)
        {
            _logger.Information("{Context}. Ошибка: {Error}", Context, removingUserChat.Error);
            return removingUserChat;
        }

        var saving = await _unitOfWork.SaveChangesAsync();
        if (saving.IsFailure)
        {
            await _unitOfWork.RollBackTransaction();
            _logger.Information("{Context}. Ошибка: {Error}", Context, saving.Error);
            return saving;
        }

        // удаление темы чата для уведомлений
        _logger.Information("{Context}. удаление чата для уведомлений.", Context);
        var removingNotificationsTheme = await RemoveNotificationThemeChat(chatId, themeId);
        if (removingNotificationsTheme.IsFailure)
        {
            _logger.Information(
                "{Context}. Ошибка: {Error}",
                Context,
                removingNotificationsTheme.Error
            );
            return removingNotificationsTheme;
        }

        saving = await _unitOfWork.SaveChangesAsync();
        if (saving.IsFailure)
        {
            await _unitOfWork.RollBackTransaction();
            _logger.Information("{Context}. Ошибка: {Error}", Context, saving.Error);
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

    // удалить тему чата для уведомлений
    private async Task<Result> RemoveNotificationThemeChat(long chatId, long themeId)
    {
        var useCase = new RemoveThemeUseCase(chatId, themeId);
        return await _notificationChats.Visit(useCase);
    }

    // удалить дочерний чат
    private async Task<Result> RemoveUserChildChat(long userId, long chatId, long themeId)
    {
        var useCase = new RemoveExternalChatThemeUseCase(userId, chatId, themeId);
        return await _userChats.Visit(useCase);
    }
}
