using CSharpFunctionalExtensions;
using PRTelegramBot.Attributes;
using PRTelegramBot.Extensions;
using PRTelegramBot.Models.Enums;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Queries.GetNotificationReceiverTimeInformation;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.GetChatTime;

/// <summary>
/// Endpoint обработки команды /bot_chat_time
/// </summary>
/// <param name="handler">
///     <inheritdoc cref="GetNotificationReceiverTimeInformationQueryHandler"/>
/// </param>
[BotHandler]
public sealed class GetChatTimeEndpoint(
    IQueryHandler<
        GetNotificationReceiverTimeInformationQuery,
        GetNotificationReceiverTimeInformationQueryResponse
    > handler,
    Serilog.ILogger logger
)
{
    private const string Context = "Endpoint получить время чата.";
    private readonly Serilog.ILogger _logger = logger;

    /// <summary>
    /// <inheritdoc cref="GetNotificationReceiverTimeInformationQueryHandler"/>
    /// </summary>
    private readonly IQueryHandler<
        GetNotificationReceiverTimeInformationQuery,
        GetNotificationReceiverTimeInformationQueryResponse
    > _handler = handler;

    /// <summary>
    /// Обработчик endpoint'а
    /// </summary>
    /// <param name="client">Telegram bot клиент для общения с telegram</param>
    /// <param name="update">Последнее событие (в данном случае вызов endpoint)</param>
    [ReplyMenuHandler(CommandComparison.Contains, "/bot_chat_time@", "/bot_chat_time")]
    public async Task GetChatTime(ITelegramBotClient client, Update update)
    {
        _logger.Information("{Context}. Вызван.", Context);
        Result<TelegramBotUser> telegramBotUserResult = update.GetUser();
        if (telegramBotUserResult.IsFailure)
        {
            await telegramBotUserResult.SendError(client, update);
            return;
        }

        Message? message = update.Message;
        if (message == null)
            return;

        int? messageThreadId = message.MessageThreadId;
        long chatId = message.Chat.Id;

        if (messageThreadId != null) // если вызывается из темы, отправка ответа в тему.
        {
            await ReplyInProcess(client, chatId, messageThreadId);
            string information = await GetChatTimeHandle(chatId);
            await client.SendMessage(
                chatId,
                text: information,
                messageThreadId: messageThreadId.Value
            );
        }
        else // если вызывается из основного чата, отправка ответа в основной чат.
        {
            await ReplyInProcess(client, chatId, messageThreadId);
            string information = await GetChatTimeHandle(chatId);
            await client.SendMessage(chatId, text: information);
        }

        update.ClearStepUserHandler();
        update.ClearCacheData();
    }

    /// <summary>
    /// Запрашивание информации о времени чата.
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <returns>Информация о времени чата.</returns>
    private async Task<string> GetChatTimeHandle(long chatId)
    {
        GetNotificationReceiverTimeInformationQuery informationQuery = new(chatId);
        GetNotificationReceiverTimeInformationQueryResponse response = await _handler.Handle(
            informationQuery
        );
        return response.Information;
    }

    /// <summary>
    /// Отправка ответа об ожидании
    /// </summary>
    /// <param name="client">Telegram bot клиент для общения с telegram</param>
    /// <param name="chatId">Id чата</param>
    /// <param name="messageThreadId">Id темы чата</param>
    private static async Task ReplyInProcess(
        ITelegramBotClient client,
        long chatId,
        int? messageThreadId = null
    )
    {
        if (messageThreadId != null)
            await client.SendMessage(
                chatId,
                text: "Запрашиваю время...",
                messageThreadId: messageThreadId.Value
            );
        else
            await client.SendMessage(chatId, text: "Запрашиваю время...");
    }
}
