using CSharpFunctionalExtensions;
using PRTelegramBot.Extensions;
using PRTelegramBot.Models;
using RocketTaskPlanner.Application.ApplicationTimeContext.Features.SaveTimeZoneDbApiKey;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Application.UsersContext.Features.AddUserWithPermissions;
using RocketTaskPlanner.Application.UsersContext.Visitor;
using RocketTaskPlanner.Domain.PermissionsContext;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotConstants;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.StartEndpoint.Handlers.OnTokenReply;

/// <summary>
/// Обработчик ответа на ввод ключа Time Zone Db.
/// </summary>
/// <param name="context">Контекст обработчиков команды /start</param>
/// <param name="saveTokenHandler">Обработчик для сохранения ключа time zone db.</param>
/// <param name="userUseCases">Посетитель для обработки команд связанных с контекстом пользователей.</param>
public sealed class OnTimeZoneDbTokenReplyHandler(
    TelegramBotExecutionContext context,
    IUseCaseHandler<SaveTimeZoneDbApiKeyUseCase, TimeZoneDbProvider> saveTokenHandler,
    IUsersUseCaseVisitor userUseCases
) : ITelegramBotHandler
{
    private readonly TelegramBotExecutionContext _context = context;

    private readonly IUseCaseHandler<
        SaveTimeZoneDbApiKeyUseCase,
        TimeZoneDbProvider
    > _saveTokenHandler = saveTokenHandler;

    private readonly IUsersUseCaseVisitor _userUseCases = userUseCases;
    public string Command => TimeZoneDbApiKeyManagementConstants.TokenReplyCommand;

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        Result<string> message = update.GetMessage();
        if (message.IsFailure)
        {
            await message.SendError(client, update);
            return;
        }

        await update.RemoveLastMessage(client);
        StepTelegram? previous = update.GetStepHandler<StepTelegram>();
        if (previous == null)
            return;

        // получение информации о пользователе из события telegram.
        Result<TelegramBotUser> userResult = update.GetUser();
        if (userResult.IsFailure)
        {
            await userResult.SendError(client, update);
            return;
        }
        TelegramBotUser user = userResult.Value;

        // создание первого пользователя с правами edit configuration и create tasks.
        Result ownerRegistration = await AddOwner(user);
        if (ownerRegistration.IsFailure)
        {
            await ownerRegistration.SendError(client, update);
            return;
        }

        // сохранение ключа time zone db.
        Result savingTimeZoneApiKey = await SaveTimeZoneDbApiKey(message.Value);
        if (savingTimeZoneApiKey.IsFailure)
        {
            await savingTimeZoneApiKey.SendError(client, update);
            return;
        }

        _context.ClearHandlers(update);
        _context.ClearCacheData(update);
        OptionMessage replyMessageOption = new() { ClearMenu = true };
        await PRTelegramBot.Helpers.Message.Send(
            client,
            update,
            TimeZoneDbApiKeyManagementConstants.ReplyMessageOnSuccess,
            replyMessageOption
        );
        await client.RegisterBotCommands();
    }

    private async Task<Result> SaveTimeZoneDbApiKey(string message)
    {
        SaveTimeZoneDbApiKeyUseCase useCase = new(message);
        Result<TimeZoneDbProvider> provider = await _saveTokenHandler.Handle(useCase);
        return provider;
    }

    private async Task<Result> AddOwner(TelegramBotUser user)
    {
        long userId = user.Id;
        string userName = user.CombineNamesAsNickname();
        string[] permissions = [PermissionNames.EditConfiguration, PermissionNames.CreateTasks];
        AddUserWithPermissionsUseCase useCase = new(userId, userName, permissions);
        return await _userUseCases.Visit(useCase);
    }
}
