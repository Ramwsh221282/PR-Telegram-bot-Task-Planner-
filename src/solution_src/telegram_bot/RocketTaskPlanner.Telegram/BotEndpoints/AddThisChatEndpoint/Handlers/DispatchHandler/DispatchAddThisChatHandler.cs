using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.HasNotificationReceiver;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.HasNotificationReceiverTheme;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotEndpoints.AddThisChatEndpoint.Handlers.AddGeneralChat;
using RocketTaskPlanner.Telegram.BotEndpoints.AddThisChatEndpoint.Handlers.AddThemeChat;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.AddThisChatEndpoint.Handlers.DispatchHandler;

/// <summary>
/// Обработчик для распознавания какой чат добавлять (тему или основной чат)
/// </summary>
/// <param name="context">Контекст выполнения обработки команды /add_this_chata</param>
/// <param name="repository">Репозиторий провайдера времени Time Zone Db</param>
/// <param name="hasReceiverHandler">Обработчик для проверки на существование чата</param>
/// <param name="hasReceiverThemeHandler">Обработчик для проверки на существование темы</param>
public sealed class DispatchAddThisChatHandler(
    TelegramBotExecutionContext context,
    IApplicationTimeRepository<TimeZoneDbProvider> repository,
    IQueryHandler<HasNotificationReceiverQuery, bool> hasReceiverHandler,
    IQueryHandler<HasNotificationReceiverThemeQuery, bool> hasReceiverThemeHandler
) : ITelegramBotHandler
{
    private readonly TelegramBotExecutionContext _context = context;
    private readonly IApplicationTimeRepository<TimeZoneDbProvider> _repository = repository;
    private readonly IQueryHandler<HasNotificationReceiverQuery, bool> _hasReceiverHandler =
        hasReceiverHandler;
    private readonly IQueryHandler<
        HasNotificationReceiverThemeQuery,
        bool
    > _hasReceiverThemeHandler = hasReceiverThemeHandler;
    public string Command => AddThisChatEndpointConstants.DispatchAddThisChatHandler;

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        Result<TimeZoneDbProvider> provider = await _repository.Get();
        if (provider.IsFailure)
        {
            await provider.SendError(client, update);
            return;
        }

        Message? message = update.Message;
        if (message == null)
            return;

        int? messageThreadId = message.MessageThreadId;
        bool isTopicMessage = message.IsTopicMessage;
        long chatId = message.Chat.Id;
        if (messageThreadId != null && isTopicMessage) // если тема
        {
            // если тема уже подписана, то ошибка
            if (await HasReceiverThemeSubscribed(chatId, messageThreadId.Value))
            {
                await ReplyReceiverThemeAlreadySubscribed(client, chatId, messageThreadId.Value);
                return;
            }

            ThemeChatCache cache = new(chatId, messageThreadId.Value);
            ITelegramBotHandler nextHandler = _context.GetRequiredHandler(
                AddThisChatEndpointConstants.ThemeChatHandler
            );
            _context.AssignNextStep(update, nextHandler, cache);

            // вызов следующего обработчика и немедленное выполнение.
            await _context.AssignAndRun(client, update, nextHandler, cache);
        }
        else
        {
            // если чат уже подписан, то ошибка
            if (await HasReceiverSubscribed(chatId))
            {
                await ReplyReceiverAlreadySubscribed(client, chatId);
                return;
            }

            string chatName = string.IsNullOrWhiteSpace(message.Chat.Title)
                ? "Unknown Chat Name"
                : message.Chat.Title;
            GeneralChatCache cache = new(provider.Value, chatId, chatName);
            ITelegramBotHandler nextHandler = _context.GetRequiredHandler(
                AddThisChatEndpointConstants.DispatchAddThisChatHandler
            );

            // вызов следующего обработчика для выбора временных зон.
            await _context.AssignAndRun(client, update, nextHandler, cache);
        }
    }

    private async Task<bool> HasReceiverSubscribed(long receiverId)
    {
        HasNotificationReceiverQuery query = new(receiverId);
        return await _hasReceiverHandler.Handle(query);
    }

    private async Task<bool> HasReceiverThemeSubscribed(long receiverId, long themeId)
    {
        HasNotificationReceiverThemeQuery query = new(receiverId, themeId);
        return await _hasReceiverThemeHandler.Handle(query);
    }

    private static async Task ReplyReceiverAlreadySubscribed(
        ITelegramBotClient handler,
        long chatId
    )
    {
        string message = $"Чат с ID: {chatId} уже подписан";
        await handler.SendMessage(chatId, text: message);
    }

    private static async Task ReplyReceiverThemeAlreadySubscribed(
        ITelegramBotClient handler,
        long chatId,
        int themeId
    )
    {
        string message = $"Тема с ID: {themeId} у чата с ID: {chatId} уже подписана";
        await handler.SendMessage(chatId, text: message, messageThreadId: themeId);
    }
}
