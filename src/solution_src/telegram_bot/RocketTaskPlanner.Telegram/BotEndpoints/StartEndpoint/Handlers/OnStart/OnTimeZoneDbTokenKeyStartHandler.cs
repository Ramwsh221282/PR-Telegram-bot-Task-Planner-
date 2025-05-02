using System.Diagnostics;
using CSharpFunctionalExtensions;
using PRTelegramBot.Models;
using PRTelegramBot.Utils;
using RocketTaskPlanner.Application.UsersContext.Contracts;
using RocketTaskPlanner.Domain.PermissionsContext;
using RocketTaskPlanner.Domain.UsersContext.ValueObjects;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Queries.HasTimeZoneDbToken;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotConstants;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RocketTaskPlanner.Telegram.BotEndpoints.StartEndpoint.Handlers.OnStart;

public sealed class OnTimeZoneDbTokenKeyStartHandler(
    TelegramBotExecutionContext context,
    IQueryHandler<HasTimeZoneDbTokenQuery, HasTimeZoneDbTokenQueryResponse> queryHandler,
    IUsersRepository usersRepository
) : ITelegramBotHandler
{
    private static readonly ReplyKeyboardMarkup Menu = MenuGenerator.ReplyKeyboard(
        1,
        [
            new KeyboardButton(ButtonTextConstants.ContinueStepButtonText),
            new KeyboardButton(ButtonTextConstants.CancelSessionButtonText),
        ]
    );

    private readonly IQueryHandler<
        HasTimeZoneDbTokenQuery,
        HasTimeZoneDbTokenQueryResponse
    > _queryHandler = queryHandler;

    private readonly TelegramBotExecutionContext _context = context;

    private readonly IUsersRepository _usersRepository = usersRepository;

    public string Command => TimeZoneDbApiKeyManagementConstants.StartCommand;

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        bool hasTimeZoneConfigured = await HasTimeZoneConfigured();
        bool containsOwner = await ContainsOwner();
        bool isCalledByOwner = await IsCalledByOwner(update);
        if (hasTimeZoneConfigured && containsOwner || !hasTimeZoneConfigured && containsOwner)
        {
            Task replyHandle = isCalledByOwner switch
            {
                true => HandleForOwner(client, update),
                false => HandleForNonOwner(client, update),
            };
            await replyHandle;
            return;
        }

        Result<string> startMessage = update.GetMessage();
        if (startMessage.IsFailure)
        {
            await startMessage.SendError(client, update);
            return;
        }

        string replyMessage = startMessage.Value switch
        {
            TimeZoneDbApiKeyManagementConstants.StartCommand =>
                TimeZoneDbApiKeyManagementConstants.ReplyMessageOnStart,
            TimeZoneDbApiKeyManagementConstants.UpdateCommand =>
                TimeZoneDbApiKeyManagementConstants.ReplyMessageOnUpdateKey,
            _ => throw new UnreachableException("Unsupported settings controller command"),
        };

        ITelegramBotHandler handler = _context.GetRequiredHandler(
            TimeZoneDbApiKeyManagementConstants.ContinueCommand
        );
        _context.AssignNextStep(update, handler);
        OptionMessage options = new() { MenuReplyKeyboardMarkup = Menu };
        await PRTelegramBot.Helpers.Message.Send(client, update, replyMessage, options);
    }

    private async Task<bool> HasTimeZoneConfigured()
    {
        HasTimeZoneDbTokenQuery query = new();
        HasTimeZoneDbTokenQueryResponse response = await _queryHandler.Handle(query);
        return response.Has;
    }

    private async Task<bool> ContainsOwner() =>
        await _usersRepository.ReadableRepository.ContainsOwner();

    private async Task<bool> IsCalledByOwner(Update update)
    {
        Result<TelegramBotUser> userInfo = update.GetUser();
        if (userInfo.IsFailure)
            return false;

        long userId = userInfo.Value.Id;

        Result<Domain.UsersContext.User> userResult =
            await _usersRepository.ReadableRepository.GetById(UserId.Create(userId));
        if (userResult.IsFailure)
            return false;

        Domain.UsersContext.User user = userResult.Value;
        return user.HasPermission(PermissionNames.EditConfiguration)
            && user.HasPermission(PermissionNames.CreateTasks);
    }

    private static async Task HandleForOwner(ITelegramBotClient client, Update update)
    {
        await client.RegisterBotStartCommands();
        await PRTelegramBot.Helpers.Message.Send(
            client,
            update,
            "Добро пожаловать назад в бот-планировщик задач."
        );
    }

    private static async Task HandleForNonOwner(ITelegramBotClient client, Update update)
    {
        await client.RegisterBotStartCommands();
        await PRTelegramBot.Helpers.Message.Send(
            client,
            update,
            "Функция доступна только обладателю."
        );
    }
}
