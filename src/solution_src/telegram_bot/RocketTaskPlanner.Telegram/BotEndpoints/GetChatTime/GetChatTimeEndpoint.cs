using PRTelegramBot.Attributes;
using PRTelegramBot.Models.Enums;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetNotificationReceiverTimeInformation;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.GetChatTime;

[BotHandler]
public sealed class GetChatTimeEndpoint(
    IQueryHandler<
        GetNotificationReceiverTimeInformationQuery,
        GetNotificationReceiverTimeInformationQueryResponse
    > handler
)
{
    private readonly IQueryHandler<
        GetNotificationReceiverTimeInformationQuery,
        GetNotificationReceiverTimeInformationQueryResponse
    > _handler = handler;

    [ReplyMenuHandler(
        CommandComparison.Contains,
        StringComparison.OrdinalIgnoreCase,
        ["/chat_time"]
    )]
    public async Task GetChatTime(ITelegramBotClient client, Update update)
    {
        Message? message = update.Message;
        if (message == null)
            return;

        int? messageThreadId = message.MessageThreadId;
        long chatId = message.Chat.Id;

        if (messageThreadId != null)
        {
            await ReplyInProcess(client, chatId, messageThreadId);
            string information = await GetChatTimeHandle(chatId);
            await client.SendMessage(
                chatId,
                text: information,
                messageThreadId: messageThreadId.Value
            );
        }
        else
        {
            await ReplyInProcess(client, chatId, messageThreadId);
            string information = await GetChatTimeHandle(chatId);
            await client.SendMessage(chatId, text: information);
        }
    }

    private async Task<string> GetChatTimeHandle(long chatId)
    {
        GetNotificationReceiverTimeInformationQuery informationQuery = new(chatId);
        GetNotificationReceiverTimeInformationQueryResponse response = await _handler.Handle(
            informationQuery
        );
        return response.Information;
    }

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
