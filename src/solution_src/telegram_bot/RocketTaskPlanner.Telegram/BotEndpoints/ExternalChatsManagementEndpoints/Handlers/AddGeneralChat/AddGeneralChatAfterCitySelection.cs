using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.Facades;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotConstants;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.ExternalChatsManagementEndpoints.Handlers.AddGeneralChat;

public sealed class AddGeneralChatAfterCitySelection(
    UserChatRegistrationFacade notFirstSignUp,
    FirstUserChatRegistrationFacade firstSignUp,
    IExternalChatsReadableRepository repository
)
{
    private readonly FirstUserChatRegistrationFacade _firstSignUp = firstSignUp;
    private readonly UserChatRegistrationFacade _notFirstSignUp = notFirstSignUp;
    private readonly IExternalChatsReadableRepository _repository = repository;

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

    private Task<Result> HandleForFirstUser(
        long userId,
        string userName,
        long chatId,
        string chatTitle,
        string chatZoneName
    ) => _firstSignUp.RegisterUser(userId, userName, chatId, chatTitle, chatZoneName);

    private Task<Result> HandleForNotFirstSignUp(
        long userId,
        long chatId,
        string chatName,
        string chatZoneName
    ) => _notFirstSignUp.AddUserExternalChat(userId, chatId, chatName, chatZoneName);
}
