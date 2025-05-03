using PRTelegramBot.Models;
using PRTelegramBot.Utils;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Queries.HasTimeZoneDbToken;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotConstants;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RocketTaskPlanner.Telegram.BotEndpoints.StartEndpoint.Handlers.OnStart;

/// <summary>
/// Начальный обработчик команды /start
/// </summary>
/// <param name="context">Контекст обработчика команды /start</param>
/// <param name="queryHandler">Обработчик проверки на существование конфигурации Time Zone Db Api Key.</param>
public sealed class OnTimeZoneDbTokenKeyStartHandler(
    TelegramBotExecutionContext context,
    IQueryHandler<HasTimeZoneDbTokenQuery, HasTimeZoneDbTokenQueryResponse> queryHandler
) : ITelegramBotHandler
{
    /// <summary>
    /// Меню с кнопками "продолжить", "отменить".
    /// </summary>
    private static readonly ReplyKeyboardMarkup Menu = MenuGenerator.ReplyKeyboard(
        1,
        [
            new KeyboardButton(ButtonTextConstants.ContinueStepButtonText),
            new KeyboardButton(ButtonTextConstants.CancelSessionButtonText),
        ]
    );

    /// <summary>
    /// Обработчик, для проверки существования конфигурации time zone db api key.
    /// </summary>
    private readonly IQueryHandler<
        HasTimeZoneDbTokenQuery,
        HasTimeZoneDbTokenQueryResponse
    > _queryHandler = queryHandler;

    /// <summary>
    /// Общий контекст, содержащий обработчики выполнения команды /start
    /// </summary>
    private readonly TelegramBotExecutionContext _context = context;

    /// <summary>
    /// Название текущего обработчика
    /// </summary>
    public string Command => TimeZoneDbApiKeyManagementConstants.StartCommand;

    /// <summary>
    /// Метод обработки команды /start
    /// </summary>
    /// <param name="client">Клиент telegram бота для общения с telegram</param>
    /// <param name="update">Последнего обновления (в данном случае вызов /start)</param>
    public async Task Handle(ITelegramBotClient client, Update update)
    {
        bool hasTimeZoneConfigured = await HasTimeZoneConfigured(); // проверка на существование конфигурации
        if (hasTimeZoneConfigured)
        {
            await HandleForConfigurationExists(client, update);
            return;
        }

        const string replyMessage = TimeZoneDbApiKeyManagementConstants.ReplyMessageOnStart; // ответ на /start.
        const string nextHandlerPath = TimeZoneDbApiKeyManagementConstants.ContinueCommand; // название следующего обработчика.

        ITelegramBotHandler handler = _context.GetRequiredHandler(nextHandlerPath); // получение следующего обработчика.
        _context.AssignNextStep(update, handler); // назначение следующего этапа - обработать кнопку "продолжить" или "отмена".

        OptionMessage options = new() { MenuReplyKeyboardMarkup = Menu };
        await PRTelegramBot.Helpers.Message.Send(client, update, replyMessage, options); // отправка ответа на команду /start
    }

    private async Task<bool> HasTimeZoneConfigured()
    {
        HasTimeZoneDbTokenQuery query = new();
        HasTimeZoneDbTokenQueryResponse response = await _queryHandler.Handle(query);
        return response.Has;
    }

    /// <summary>
    /// Отправка сообщения, если конфигурация Time Zone Db Api Key присутствует.
    /// </summary>
    /// <param name="client">Telegram bot клиент для общения с telegram</param>
    /// <param name="update">Последнее событие</param>
    private static async Task HandleForConfigurationExists(ITelegramBotClient client, Update update)
    {
        const string reply = TimeZoneDbApiKeyManagementConstants.ReplyOnTimeZoneDbConfigured;
        await PRTelegramBot.Helpers.Message.Send(client, update, reply);
    }
}
