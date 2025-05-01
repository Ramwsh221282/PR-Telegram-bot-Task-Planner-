using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterTheme;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Telegram.BotAbstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.AddThisChatEndpoint.Handlers.AddThemeChat;

public sealed class AddThemeChatHandler(
    TelegramBotExecutionContext context,
    IUseCaseHandler<RegisterThemeUseCase, RegisterThemeResponse> useCaseHandler
) : ITelegramBotHandler
{
    private readonly TelegramBotExecutionContext _context = context;
    public string Command => AddThisChatEndpointConstants.ThemeChatHandler;
    private readonly IUseCaseHandler<RegisterThemeUseCase, RegisterThemeResponse> _useCaseHandler =
        useCaseHandler;

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        Result<ThemeChatCache> cache = _context.GetCacheInfo<ThemeChatCache>();
        if (cache.IsFailure)
            return;

        long chatId = cache.Value.ChatId;
        long themeId = cache.Value.ThemeId;

        RegisterThemeUseCase useCase = new(chatId, themeId);
        Result<RegisterThemeResponse> response = await _useCaseHandler.Handle(useCase);

        if (response.IsFailure)
        {
            if (response.Error.Contains("не был найден"))
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
            else
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
            text: response.Value.Information()
        );
    }
}
