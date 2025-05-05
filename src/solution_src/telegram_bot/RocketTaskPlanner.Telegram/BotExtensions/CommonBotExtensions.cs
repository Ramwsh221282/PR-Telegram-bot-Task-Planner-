using CSharpFunctionalExtensions;
using PRTelegramBot.Extensions;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotConstants;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotExtensions;

/// <summary>
/// Extension методы для работы с telegram ботом
/// </summary>
public static class CommonBotExtensions
{
    /// <summary>
    /// Extension метод для отправки ошибки из Result Failure
    /// </summary>
    /// <param name="result">Result</param>
    /// <param name="client">Telegram bot клиент для общения с Telegram</param>
    /// <param name="update">Последнее событие</param>
    public static async Task SendError(this Result result, ITelegramBotClient client, Update update)
    {
        if (result.IsSuccess)
            return;

        long chatId = update.GetChatId();
        int? themeId = update.Message?.MessageThreadId;
        string text = $"""
            Ошибка:
            {result.Error}
            """;

        await client.SendError(chatId, themeId, text);
    }

    private static async Task SendError(
        this ITelegramBotClient client,
        long chatId,
        int? themeId,
        string text
    )
    {
        Task handling =
            themeId == null
                ? client.SendMessage(chatId: chatId, text: text)
                : client.SendMessage(chatId: chatId, text: text, messageThreadId: themeId.Value);
        await handling;
    }

    /// <summary>
    /// Extension метод для отправки ошибки из Result T Failure
    /// </summary>
    /// <param name="result">Result</param>
    /// <param name="client">Telegram bot клиент для общения с Telegram</param>
    /// <param name="update">Последнее событие</param>
    /// <typeparam name="T">Параметр из Result T </typeparam>
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

        long chatId = update.GetChatId();
        Result<int> themeId = update.GetThemeId();

        Task sendOperation = themeId.IsSuccess switch
        {
            true => client.SendMessage(chatId: chatId, text: text, messageThreadId: themeId.Value),
            false => client.SendMessage(chatId: chatId, text: text),
        };

        await sendOperation;
    }

    /// <summary>
    /// Extension метод для получения ID темы из сообщения
    /// </summary>
    /// <param name="update">Событие</param>
    /// <returns>Result int из темы чата, Failure если сообщение не из темы чата</returns>
    public static Result<int> GetThemeId(this Update update)
    {
        var message = update.Message;
        if (message == null)
            return Result.Failure<int>("Unable to get theme id");

        var themeId = message.MessageThreadId;
        return themeId == null ? Result.Failure<int>("Unable to get theme id") : themeId.Value;
    }

    /// <summary>
    /// Extension метод для получения сообщения из события
    /// </summary>
    /// <param name="update">Последнее событие</param>
    /// <returns>Result Success с сообщением, либо Failure</returns>
    public static Result<string> GetMessage(this Update update)
    {
        Message? message = update.Message;

        if (message == null)
            return Result.Failure<string>("Сообщение пустое.");

        if (message.Text == null)
            return Result.Failure<string>("Сообщение пустое.");

        return message.Text;
    }

    /// <summary>
    /// Получение Id пользователя из последнего события
    /// </summary>
    /// <param name="update">Последнее событие</param>
    /// <returns>Id пользователя</returns>
    public static Result<long> GetUserId(this Update update)
    {
        Message? message = update.Message;
        if (message == null)
            return Result.Failure<long>("ID пользователя неизвестно");
        if (message.From == null)
            return Result.Failure<long>("ID пользователя неизвестно");
        return message.From.Id;
    }

    /// <summary>
    /// Метод для удаления последнего сообщения
    /// </summary>
    /// <param name="update">Последнее событие</param>
    /// <param name="client">Telegram bot клиент для общения с telegram</param>
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

    /// <summary>
    /// Extension метод для удаления сообщения по его Id
    /// </summary>
    /// <param name="client">Telegram bot клиент для общения с Telegram</param>
    /// <param name="message">Сообщение</param>
    public static async Task RemoveMessageById(this ITelegramBotClient client, Message message)
    {
        long chatId = message.Chat.Id;
        int messageId = message.MessageId;
        await client.DeleteMessage(chatId, messageId);
    }

    /// <summary>
    /// Extension метод для получение информации пользователя из сообщения.
    /// </summary>
    /// <param name="update">Последнее событие</param>
    /// <returns>Result Success с информацей о пользователе Telegram, либо Failure</returns>
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

    /// <summary>
    /// Extension метод для получение информации пользователя из callback (например из нажатия в inline меню).
    /// </summary>
    /// <param name="update">Последнее событие</param>
    /// <returns>Result Success с информацей о пользователе Telegram, либо Failure</returns>
    public static Result<TelegramBotUser> GetUserFromCallback(this Update update)
    {
        var callback = update.CallbackQuery;
        if (callback == null)
            return Result.Failure<TelegramBotUser>(
                "Can't get user information from telegram bot update."
            );

        var userInfo = callback.From;
        long id = userInfo.Id;
        string name = userInfo.FirstName;
        string? lastName = userInfo.LastName;
        return new TelegramBotUser(id, name, lastName);
    }

    /// <summary>
    /// Отправка сообщения, что операция отменена
    /// </summary>
    /// <param name="client">Telegram bot клиент для общения с telegram</param>
    /// <param name="update">Последнее событие</param>
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
