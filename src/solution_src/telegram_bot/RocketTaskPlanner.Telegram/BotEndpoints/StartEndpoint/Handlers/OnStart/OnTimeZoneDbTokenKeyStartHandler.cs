using System.Diagnostics;
using CSharpFunctionalExtensions;
using PRTelegramBot.Models;
using PRTelegramBot.Utils;
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
    IQueryHandler<HasTimeZoneDbTokenQuery, HasTimeZoneDbTokenQueryResponse> queryHandler
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

    public string Command => TimeZoneDbApiKeyManagementConstants.StartCommand;

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        HasTimeZoneDbTokenQuery query = new();
        HasTimeZoneDbTokenQueryResponse response = await _queryHandler.Handle(query);
        if (response.Has)
        {
            await HandleIfTokenExists(client, update);
            return;
        }

        Result<string> startMessage = update.GetMessage();
        if (startMessage.IsFailure)
            return;

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

    private async Task HandleIfTokenExists(ITelegramBotClient client, Update update)
    {
        await client.RegisterBotStartCommands();
        await PRTelegramBot.Helpers.Message.Send(
            client,
            update,
            "Добро пожаловать назад в бот-планировщик задач 👋"
        );
    }
}
