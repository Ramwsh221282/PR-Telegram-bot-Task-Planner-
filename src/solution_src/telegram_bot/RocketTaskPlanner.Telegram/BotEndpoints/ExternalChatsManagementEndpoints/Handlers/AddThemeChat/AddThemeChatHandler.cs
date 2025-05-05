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
    UserThemeRegistrationFacade facade
) : ITelegramBotHandler
{
    private readonly TelegramBotExecutionContext _context = context;
    private readonly UserThemeRegistrationFacade _facade = facade;
    public string Command => AddThisChatEndpointConstants.ThemeChatHandler;

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        Result<ThemeChatCache> cache = _context.GetCacheInfo<ThemeChatCache>();
        if (cache.IsFailure)
            return;

        long userId = cache.Value.UserId;
        string chatTitle = cache.Value.ChatName;
        long chatId = cache.Value.ChatId;
        long themeId = cache.Value.ThemeId;

        Result adding = await _facade.RegisterUserTheme(chatId, themeId, userId, chatTitle);
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
