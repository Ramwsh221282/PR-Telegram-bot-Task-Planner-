using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.Facades;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.ExternalChatsManagementEndpoints.Handlers.AddThemeChat;

/// <summary>
/// Обработчик для добавления темы чата
/// </summary>
/// <param name="context">Контекст обработки команды /add_this_chat</param>
/// <param name="facade">Фасадный класс для обработки транзакци добавления пользовательского дочернего чата и темы чата для уведомления</param>
public sealed class AddThemeChatHandler(
    TelegramBotExecutionContext context,
    IServiceScopeFactory scopeFactory
) : ITelegramBotHandler
{
    /// <summary>
    /// <inheritdoc cref="TelegramBotExecutionContext"/>
    /// </summary>
    private readonly TelegramBotExecutionContext _context = context;

    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    /// <summary>
    /// <inheritdoc cref="ITelegramBotHandler.Command"/>
    /// </summary>
    public string Command => AddThisChatEndpointConstants.ThemeChatHandler;

    /// <summary>
    /// Логика обработки добавления темы чата и дочернего чата пользователя
    /// </summary>
    /// <param name="client">Telegram bot client для общения с телеграм</param>
    /// <param name="update">Последнее событие</param>

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        Result<ThemeChatCache> cache = _context.GetCacheInfo<ThemeChatCache>();
        if (cache.IsFailure)
            return;

        long userId = cache.Value.UserId;
        string chatTitle = cache.Value.ChatName;
        long chatId = cache.Value.ChatId;
        long themeId = cache.Value.ThemeId;

        await using var scope = _scopeFactory.CreateAsyncScope();
        var facade = scope.ServiceProvider.GetRequiredService<UserThemeRegistrationFacade>();
        Result adding = await facade.RegisterUserTheme(chatId, themeId, userId, chatTitle);
        if (adding.IsFailure)
        {
            await adding.SendError(client, update);
            return;
        }

        await client.SendMessage(
            chatId: chatId,
            messageThreadId: (int)themeId,
            text: "Тема чата добавлена"
        );
    }
}
