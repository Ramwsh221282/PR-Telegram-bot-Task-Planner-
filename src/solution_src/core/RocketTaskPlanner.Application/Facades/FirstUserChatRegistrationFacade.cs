using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChat;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChatOwner;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;
using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterChat;
using RocketTaskPlanner.Application.NotificationsContext.Visitor;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;

namespace RocketTaskPlanner.Application.Facades;

/// <summary>
/// Фасадный класс для добавления пользователя при первой регистрации.
/// </summary>
public sealed class FirstUserChatRegistrationFacade
{
    private const string Context = nameof(FirstUserChatRegistrationFacade);
    
    /// <summary>
    /// Unit of work для управления транзакцией
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Бизнес логика по работе с пользователем и пользовательскими чатами
    /// </summary>
    private readonly IExternalChatUseCasesVisitor _externalChatUseCases;

    /// <summary>
    /// Бизнес логика для работы с чатами для уведомлений
    /// </summary>
    private readonly INotificationUseCaseVisitor _notificationChatUseCases;

    private readonly Serilog.ILogger _logger;

    public FirstUserChatRegistrationFacade(
        IUnitOfWork unitOfWork,
        IExternalChatUseCasesVisitor externalChatUseCases,
        INotificationUseCaseVisitor notificationChatUseCases,
        Serilog.ILogger logger
    )
    {
        _unitOfWork = unitOfWork;
        _externalChatUseCases = externalChatUseCases;
        _notificationChatUseCases = notificationChatUseCases;
        _logger = logger;
    }

    /// <summary>
    /// Транзакция добавления пользователя, пользовательского чата и чата уведомлений.
    /// Транзакция для первой регистрации пользователя.
    /// Для добавления темы чата использовать UserThemeChatRegistrationFacade.
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="userName">Имя пользователя</param>
    /// <param name="chatId">Id основного чата</param>
    /// <param name="chatName">Название основного чата</param>
    /// <param name="zoneName">Название временной зоны</param>
    /// <returns></returns>
    public async Task<Result> RegisterUser(
        long userId,
        string userName,
        long chatId,
        string chatName,
        string zoneName
    )
    {
        await _unitOfWork.BeginTransaction();
        
        // регистрация пользователя
        _logger.Information("{Context} registering user.", Context);
        var user = await RegisterUser(userId, userName);
        if (user.IsFailure)
        {
            _logger.Error("{Context} registering user error: {Error}", Context, user.Error);
            return user;
        }

        var saving = await _unitOfWork.SaveChangesAsync();
        if (saving.IsFailure)
        {
            await _unitOfWork.RollBackTransaction();
            _logger.Error("{Context} registering user error: {Error}", Context, user.Error);
            return user;
        }
        
        // добавление чата пользователя
        _logger.Information("{Context} Registering user chat.", Context);
        var mainChat = await RegisterUserChat(userId, chatId, chatName);
        if (mainChat.IsFailure)
        {
            _logger.Error("{Context} Registering user chat error: {Error}", Context, mainChat.Error);
            return mainChat;
        }
        
        saving = await _unitOfWork.SaveChangesAsync();
        if (saving.IsFailure)
        {
            await _unitOfWork.RollBackTransaction();
            _logger.Error("{Context} Registering user chat error: {Error}", Context, mainChat.Error);
            return mainChat;
        }
        
        
        // добавление чата для уведомлений
        _logger.Information("{Context} Registering user notification chat.", Context);
        var notificationsChat = await RegisterNotificationChat(chatId, chatName, zoneName);
        if (notificationsChat.IsFailure)
        {
            _logger.Error("{Context} registering user notification chat error: {Error}", Context, notificationsChat.Error);
            return notificationsChat;
        }
        
        saving = await _unitOfWork.SaveChangesAsync();
        if (saving.IsFailure)
        {
            await _unitOfWork.RollBackTransaction();
            _logger.Error("{Context} registering user notification chat error: {Error}", Context, notificationsChat.Error);
            return mainChat;
        }

        var committing = await _unitOfWork.CommitTransaction();
        if (committing.IsFailure)
        {
            _logger.Fatal("{Context} failed. Error: {Error}", Context, committing.Error);
        }
        else
        {
            _logger.Information("{Context} finished.", Context);
        }
        
        return committing;
    }

    // регистрация пользователя
    private async Task<Result> RegisterUser(long userId, string userName)
    {
        AddExternalChatOwnerUseCase useCase = new(userId, userName);
        return await _externalChatUseCases.Visit(useCase);
    }

    // регистрация пользовательского чата как основного
    private async Task<Result> RegisterUserChat(long userId, long chatId, string chatName)
    {
        AddExternalChatUseCase useCase = new(userId, chatId, chatName);
        return await _externalChatUseCases.Visit(useCase);
    }

    // регистрация основного чата, как чата, куда нужно слать уведомления
    private async Task<Result> RegisterNotificationChat(
        long chatId,
        string chatName,
        string zoneName
    )
    {
        RegisterChatUseCase useCase = new(chatId, chatName, zoneName);
        return await _notificationChatUseCases.Visit(useCase);
    }
}
