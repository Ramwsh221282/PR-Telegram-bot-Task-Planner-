using PRTelegramBot.Extensions;
using PRTelegramBot.Models;
using RocketTaskPlanner.Telegram.BotAbstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.CreateTask.Handlers;

public sealed class StartHandler(TelegramBotExecutionContext context) : ITelegramBotHandler
{
    private readonly TelegramBotExecutionContext _context = context;
    public string Command => CreateTaskConstants.Starter;

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        Message? message = update.Message;
        if (message == null)
            return;
        long chatId = message.Chat.Id;
        int? threadId = message.MessageThreadId;
        CreateTaskCache cache = new(chatId, threadId);
        ITelegramBotHandler next = _context.GetRequiredHandler(CreateTaskConstants.Dispatcher);
        _context.AssignNextStep(update, next, cache);
        await ReplyCommandInvoked(client, chatId, threadId);
        StepTelegram? handler = update.GetStepHandler<StepTelegram>();
        handler!.IgnoreBasicCommands = true;
    }

    private static async Task ReplyCommandInvoked(
        ITelegramBotClient client,
        long chatId,
        int? threadId
    )
    {
        if (threadId != null)
            await client.SendMessage(
                chatId,
                text: "Введите текст уведомления.",
                messageThreadId: threadId
            );
        else
            await client.SendMessage(chatId, text: "Введите текст уведомления.");
    }
}
