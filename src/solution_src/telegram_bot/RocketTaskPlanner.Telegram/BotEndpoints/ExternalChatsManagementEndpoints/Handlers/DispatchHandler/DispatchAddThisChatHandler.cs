using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotEndpoints.ExternalChatsManagementEndpoints.Handlers.AddGeneralChat;
using RocketTaskPlanner.Telegram.BotEndpoints.ExternalChatsManagementEndpoints.Handlers.AddThemeChat;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.ExternalChatsManagementEndpoints.Handlers.DispatchHandler;

/// <summary>
/// Обработчик для распознавания какой чат добавлять (тему или основной чат)
/// Так же в нём происходит валидация на дубликаты чатов по ID, и наличие Time Zone Db Provider.
/// </summary>
public sealed class DispatchAddThisChatHandler : ITelegramBotHandler
{
    private readonly TelegramBotExecutionContext _context;
    private readonly IApplicationTimeRepository<TimeZoneDbProvider> _timeRepository;
    private readonly IExternalChatsReadableRepository _chatsRepository;

    public string Command => AddThisChatEndpointConstants.DispatchAddThisChatHandler;

    public DispatchAddThisChatHandler(
        TelegramBotExecutionContext context,
        IApplicationTimeRepository<TimeZoneDbProvider> timeRepository,
        IExternalChatsReadableRepository chatsRepository
    )
    {
        _context = context;
        _timeRepository = timeRepository;
        _chatsRepository = chatsRepository;
    }

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        // Time Zone Db Provider
        Result<TimeZoneDbProvider> provider = await _timeRepository.Get();
        if (provider.IsFailure)
        {
            await provider.SendError(client, update);
            return;
        }

        Message? message = update.Message;
        if (message == null)
            return;

        if (string.IsNullOrWhiteSpace(message.Chat.Title))
            return;

        Result<TelegramBotUser> user = update.GetUser();
        if (user.IsFailure)
            return;

        int? themeId = message.MessageThreadId;
        bool isTopic = message.IsTopicMessage;
        long chatId = message.Chat.Id;
        string chatTitle = message.Chat.Title;

        // валидация проверки дубликата чата
        if (themeId != null && isTopic)
        {
            var hasThemeDuplicate = await HasThemeDuplicate(chatId, themeId, isTopic);
            if (hasThemeDuplicate.IsFailure)
            {
                await hasThemeDuplicate.SendError(client, update);
                return;
            }
        }

        // Валидация на проверку дубликата чата
        if (themeId == null || !isTopic)
        {
            var hasChatDuplicate = await HasChatDuplicate(chatId);
            if (hasChatDuplicate.IsFailure)
            {
                await hasChatDuplicate.SendError(client, update);
                return;
            }
        }

        await Handle(
            client,
            update,
            provider.Value,
            chatId,
            chatTitle,
            themeId,
            isTopic,
            user.Value
        );
    }

    private async Task Handle(
        ITelegramBotClient client,
        Update update,
        TimeZoneDbProvider provider,
        long chatId,
        string chatTitle,
        int? themeId,
        bool isTopicMessage,
        TelegramBotUser user
    )
    {
        Task handling =
            (!isTopicMessage || themeId == null)
                ? HandleForChat(client, update, provider, chatId, chatTitle)
                : HandleForTheme(client, update, chatId, themeId.Value, chatTitle, user);
        await handling;
    }

    // обработать для чата
    private async Task HandleForChat(
        ITelegramBotClient client,
        Update update,
        TimeZoneDbProvider provider,
        long chatId,
        string chatTitle
    )
    {
        var cache = new GeneralChatCache(provider, chatId, chatTitle);
        const string nextHandlerName = AddThisChatEndpointConstants.DispatchAddThisChatHandler;
        var handler = _context.GetRequiredHandler(nextHandlerName);
        await _context.AssignAndRun(client, update, handler, cache);
    }

    // обработать для темы чата
    private async Task HandleForTheme(
        ITelegramBotClient client,
        Update update,
        long chatId,
        int themeId,
        string chatTitle,
        TelegramBotUser user
    )
    {
        var cache = new ThemeChatCache(chatId, themeId, user.Id, chatTitle);
        const string nextHandleName = AddThisChatEndpointConstants.ThemeChatHandler;
        var nextHandler = _context.GetRequiredHandler(nextHandleName);
        _context.AssignNextStep(update, nextHandler, cache);
        await _context.AssignAndRun(client, update, nextHandler, cache);
    }

    private async Task<Result> HasThemeDuplicate(long chatId, int? themeId, bool isTopicMessage)
    {
        if (themeId == null || !isTopicMessage)
            return Result.Success();

        if (await _chatsRepository.ContainsChildChat(chatId, themeId.Value))
            return Result.Failure($"Тема чата: {chatId} {themeId} уже добавлена");

        return Result.Success();
    }

    private async Task<Result> HasChatDuplicate(long chatId)
    {
        if (await _chatsRepository.ContainsChat(chatId))
            return Result.Failure($"Чат: {chatId} уже добавлен");

        return Result.Success();
    }
}
