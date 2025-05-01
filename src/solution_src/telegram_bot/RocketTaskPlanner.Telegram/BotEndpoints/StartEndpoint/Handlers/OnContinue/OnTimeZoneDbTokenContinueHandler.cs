using CSharpFunctionalExtensions;
using PRTelegramBot.Extensions;
using PRTelegramBot.Models;
using PRTelegramBot.Utils;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotConstants;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RocketTaskPlanner.Telegram.BotEndpoints.StartEndpoint.Handlers.OnContinue;

public sealed class OnTimeZoneDbTokenContinueHandler(TelegramBotExecutionContext context)
    : ITelegramBotHandler
{
    private static readonly ReplyKeyboardMarkup Menu = MenuGenerator.ReplyKeyboard(
        1,
        [new KeyboardButton(ButtonTextConstants.CancelSessionButtonText)]
    );

    private readonly TelegramBotExecutionContext _context = context;
    public string Command => TimeZoneDbApiKeyManagementConstants.ContinueCommand;

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        Result<string> message = update.GetMessage();
        if (message.IsFailure)
            return;

        await update.RemoveLastMessage(client);

        StepTelegram? previous = update.GetStepHandler<StepTelegram>();
        if (previous == null)
            return;

        if (message.Value == TimeZoneDbApiKeyManagementConstants.CancelCommand)
        {
            await client.SendOperationCancelledReply(update);
            _context.ClearHandlers(update);
            _context.ClearCacheData(update);
            return;
        }

        if (message.Value != ButtonTextConstants.ContinueStepButtonText)
        {
            _context.AssignNextStep(update, this);
            return;
        }

        ITelegramBotHandler next = _context.GetRequiredHandler(
            TimeZoneDbApiKeyManagementConstants.TokenReplyCommand
        );

        _context.AssignNextStep(update, next);

        OptionMessage options = new() { ClearMenu = true };
        options.ClearMenu = false;
        options.MenuReplyKeyboardMarkup = Menu;

        await PRTelegramBot.Helpers.Message.Send(
            client,
            update,
            TimeZoneDbApiKeyManagementConstants.ReplyMessageOnContinue,
            options
        );
    }
}
