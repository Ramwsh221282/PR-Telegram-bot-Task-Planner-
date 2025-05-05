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

    public FirstUserChatRegistrationFacade(
        IUnitOfWork unitOfWork,
        IExternalChatUseCasesVisitor externalChatUseCases,
        INotificationUseCaseVisitor notificationChatUseCases
    )
    {
        _unitOfWork = unitOfWork;
        _externalChatUseCases = externalChatUseCases;
        _notificationChatUseCases = notificationChatUseCases;
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
        using (_unitOfWork)
        {
            Result user = await RegisterUser(userId, userName);
            if (user.IsFailure)
                return user;

            Result mainChat = await RegisterUserChat(userId, chatId, chatName);
            if (mainChat.IsFailure)
                return mainChat;

            Result notificationsChat = await RegisterNotificationChat(chatId, chatName, zoneName);
            if (notificationsChat.IsFailure)
                return notificationsChat;

            await _unitOfWork.Process();
            Result saving = _unitOfWork.TryCommit();
            if (saving.IsFailure)
                return saving;
        }

        return Result.Success();
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
