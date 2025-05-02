using CSharpFunctionalExtensions;
using PRTelegramBot.Extensions;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotConstants;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotExtensions;

public static class CommonBotExtensions
{
    public static async Task SendError(this Result result, ITelegramBotClient client, Update update)
    {
        if (result.IsSuccess)
            return;

        string text = $"""
            Ошибка:
            {result.Error}
            """;

        await PRTelegramBot.Helpers.Message.Send(client, update, text);
    }

    public static async Task SendError<T>(
        this Result<T> result,
        ITelegramBotClient client,
        Update update
    )
    {
        if (result.IsSuccess)
            return;

        string text = $"""
            Ошибка:
            {result.Error}
            """;

        await PRTelegramBot.Helpers.Message.Send(client, update, text);
    }

    public static Result<string> GetMessage(this Update update)
    {
        Message? message = update.Message;

        if (message == null)
            return Result.Failure<string>("Сообщение пустое.");

        if (message.Text == null)
            return Result.Failure<string>("Сообщение пустое.");

        return message.Text;
    }

    public static Result<long> GetUserId(this Update update)
    {
        Message? message = update.Message;
        if (message == null)
            return Result.Failure<long>("ID пользователя неизвестно");
        if (message.From == null)
            return Result.Failure<long>("ID пользователя неизвестно");
        return message.From.Id;
    }

    public static async Task RemoveLastMessage(this Update update, ITelegramBotClient client)
    {
        try
        {
            Message? message = update.Message;
            if (message == null)
                return;

            int messageId = message.MessageId;
            await client.DeleteMessage(update.GetChatId(), messageId);
        }
        catch
        {
            // ignored
        }
    }

    public static async Task RemoveMessageById(this ITelegramBotClient client, Message message)
    {
        long chatId = message.Chat.Id;
        int messageId = message.MessageId;
        await client.DeleteMessage(chatId, messageId);
    }

    public static Result<TelegramBotUser> GetUser(this Update update)
    {
        var message = update.Message;
        if (message == null)
            return Result.Failure<TelegramBotUser>(
                "Can't get user information from telegram bot update."
            );

        var userInfo = message.From;
        if (userInfo == null)
            return Result.Failure<TelegramBotUser>(
                "Can't get user information from telegram bot update."
            );

        long id = userInfo.Id;
        string name = userInfo.FirstName;
        string? lastName = userInfo.LastName;
        return new TelegramBotUser(id, name, lastName);
    }

    public static async Task SendOperationCancelledReply(
        this ITelegramBotClient client,
        Update update
    ) =>
        await PRTelegramBot.Helpers.Message.Send(
            client,
            update,
            ReplyMessageConstants.OperationCanceled
        );
}
