using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.Facades;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotConstants;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.ExternalChatsManagementEndpoints.Handlers.AddGeneralChat;

/// <summary>
/// Обработчик добавления основного чата после выбора временной зоны.
/// <param name="notFirstSignUp">
///     <inheritdoc cref="UserChatRegistrationFacade"/>
/// </param>
/// <param name="firstSignUp">
///     <inheritdoc cref="FirstUserChatRegistrationFacade"/>
/// </param>
/// <param name="repository">
///     <inheritdoc cref="IExternalChatsReadableRepository"/>
/// </param>
/// /// </summary>
public sealed class AddGeneralChatAfterCitySelection(
    IServiceScopeFactory scopeFactory,
    IExternalChatsReadableRepository repository
)
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    /// <summary>
    /// <inheritdoc cref="IExternalChatsReadableRepository"/>
    /// </summary>
    private readonly IExternalChatsReadableRepository _repository = repository;

    /// <summary>
    /// Логика добавления основного чата.
    /// Добавляет и регистрирует чат, если пользователь вызывает эту команду впервые.
    /// Добавляет новый основной чат пользователю, если существующий пользователь вызывает эту команду.
    /// </summary>
    /// <param name="selector">
    ///     <inheritdoc cref="AddGeneralChatCitiesEnum"/>
    /// </param>
    /// <param name="client">
    ///     Telegram bot для взаимодействия с Telegram
    /// </param>
    /// <param name="update">Последнее событие</param>
    public async Task Handle(
        AddGeneralChatCitiesEnum selector,
        ITelegramBotClient client,
        Update update
    )
    {
        Result<TelegramBotUser> userInfoResult = update.GetUserFromCallback();
        if (userInfoResult.IsFailure)
        {
            await userInfoResult.SendError(client, update);
            return;
        }

        TelegramBotUser user = userInfoResult.Value;
        var hasUserRegistered = await _repository.HasUserRegistered(user.Id);
        var userId = user.Id;
        var userName = user.CombineNamesAsNickname();
        var information = AddGeneralChatScope.GetScopeInfo(selector);
        (long chatId, string chatName, string zoneName) = information.AsUseCase();

        var handling = await DispatchHandle(
            userId,
            userName,
            chatId,
            chatName,
            zoneName,
            hasUserRegistered
        );

        if (handling.IsFailure)
        {
            await handling.SendError(client, update);
            return;
        }

        await PRTelegramBot.Helpers.Message.Send(client, update, $"Чат подписан.");
        await client.RegisterBotCommands();
    }

    private Task<Result> DispatchHandle(
        long userId,
        string userName,
        long chatId,
        string chatTitle,
        string chatZoneName,
        bool isRegistered
    ) =>
        isRegistered
            ? HandleForNotFirstSignUp(userId, chatId, chatTitle, chatZoneName)
            : HandleForFirstUser(userId, userName, chatId, chatTitle, chatZoneName);

    private async Task<Result> HandleForFirstUser(
        long userId,
        string userName,
        long chatId,
        string chatTitle,
        string chatZoneName
    )
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var facade = scope.ServiceProvider.GetRequiredService<FirstUserChatRegistrationFacade>();
        return await facade.RegisterUser(userId, userName, chatId, chatTitle, chatZoneName);
    }

    private async Task<Result> HandleForNotFirstSignUp(
        long userId,
        long chatId,
        string chatName,
        string chatZoneName
    )
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var facade = scope.ServiceProvider.GetRequiredService<UserChatRegistrationFacade>();
        return await facade.AddUserExternalChat(userId, chatId, chatName, chatZoneName);
    }
}
