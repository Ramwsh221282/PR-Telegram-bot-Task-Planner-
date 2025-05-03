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

/// <summary>
/// Обработчик продолжения конфигурирования Time Zone Db Api Key, при вызове /start
/// </summary>
/// <param name="context">Контекст обработки команды /start, содержащий обработчики.</param>
public sealed class OnTimeZoneDbTokenContinueHandler(TelegramBotExecutionContext context)
    : ITelegramBotHandler
{
    /// <summary>
    /// Меню с кнопкой "отменить"
    /// </summary>
    private static readonly ReplyKeyboardMarkup Menu = MenuGenerator.ReplyKeyboard(
        1,
        [new KeyboardButton(ButtonTextConstants.CancelSessionButtonText)]
    );

    /// <summary>
    /// Контекст обработки команды /start, содержащий обработчики.
    /// </summary>
    private readonly TelegramBotExecutionContext _context = context;

    /// <summary>
    /// Название текущего обработчика.
    /// </summary>
    public string Command => TimeZoneDbApiKeyManagementConstants.ContinueCommand;

    /// <summary>
    /// Метод обработки, если была нажата кнопка "продолжить"
    /// </summary>
    /// <param name="client">Telegram bot клиент для общения с Telegram</param>
    /// <param name="update">Последнее событие (в данном случае, нажатие на кнопку продолжить).</param>
    public async Task Handle(ITelegramBotClient client, Update update)
    {
        Result<string> message = update.GetMessage();
        if (message.IsFailure)
            return;

        await update.RemoveLastMessage(client);

        // если не из предыдущего шага (вызова /start).
        StepTelegram? previous = update.GetStepHandler<StepTelegram>();
        if (previous == null)
            return;

        // если была вызвана отмена - очистка контекста и кеша.
        if (message.Value == TimeZoneDbApiKeyManagementConstants.CancelCommand)
        {
            await client.SendOperationCancelledReply(update);
            _context.ClearHandlers(update);
            _context.ClearCacheData(update);
            return;
        }

        // если кнопка не "продолжить", повторный вызов текущего обработчиика.
        if (message.Value != ButtonTextConstants.ContinueStepButtonText)
        {
            _context.AssignNextStep(update, this);
            return;
        }

        // назначение обработчика, ожидающего ввода ключа.
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
