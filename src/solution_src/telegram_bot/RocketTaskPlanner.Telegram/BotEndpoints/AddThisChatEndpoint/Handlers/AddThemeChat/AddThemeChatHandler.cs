using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterTheme;
using RocketTaskPlanner.Application.NotificationsContext.Visitor;
using RocketTaskPlanner.Telegram.BotAbstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.AddThisChatEndpoint.Handlers.AddThemeChat;

/// <summary>
/// Обработчик для добавления темы чата
/// </summary>
/// <param name="context">Контекст обработки команды /add_this_chat</param>
/// <param name="notificationUseCases">Посетитель для выполнения команд в контексте уведомлений</param>
public sealed class AddThemeChatHandler(
    TelegramBotExecutionContext context,
    INotificationUseCaseVisitor notificationUseCases
) : ITelegramBotHandler
{
    private readonly TelegramBotExecutionContext _context = context;
    public string Command => AddThisChatEndpointConstants.ThemeChatHandler;
    private readonly INotificationUseCaseVisitor _notificationUseCases = notificationUseCases;

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        Result<ThemeChatCache> cache = _context.GetCacheInfo<ThemeChatCache>();
        if (cache.IsFailure)
            return;

        long chatId = cache.Value.ChatId;
        long themeId = cache.Value.ThemeId;

        RegisterThemeUseCase useCase = new(chatId, themeId);
        Result response = await _notificationUseCases.Visit(useCase);

        if (response.IsFailure)
        {
            if (response.Error.Contains("не был найден")) // если не был найден основной чат.
            {
                await client.SendMessage(
                    chatId: chatId,
                    messageThreadId: (int)themeId,
                    text: """
                    Ошибка.
                    Возможно чат, для которого Вы собираетесь подписать тему, ещё не подписан.
                    """
                );
            }
            else // для прочих ошибок
            {
                await client.SendMessage(
                    chatId: chatId,
                    messageThreadId: (int)themeId,
                    text: response.Error
                );
            }
            return;
        }

        await client.SendMessage(
            chatId: chatId,
            messageThreadId: (int)themeId,
            text: "Тема чата добавлена"
        );
    }
}
