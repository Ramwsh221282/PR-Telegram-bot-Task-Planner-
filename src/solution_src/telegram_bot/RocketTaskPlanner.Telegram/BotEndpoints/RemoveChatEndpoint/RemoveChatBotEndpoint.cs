using CSharpFunctionalExtensions;
using PRTelegramBot.Attributes;
using PRTelegramBot.Extensions;
using PRTelegramBot.Models.Enums;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.Facades;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.RemoveChatEndpoint;

/// <summary>
/// Endpoint обработки удаления чата.
/// </summary>
[BotHandler]
public sealed class RemoveChatBotEndpoint
{
    private const string Context = nameof(RemoveChatBotEndpoint);
    private readonly Serilog.ILogger _logger;
    
    /// <summary>
    /// <inheritdoc cref="IExternalChatsReadableRepository"/>
    /// </summary>
    private readonly IExternalChatsReadableRepository _repository;

    private readonly IServiceScopeFactory _scopeFactory;

    public RemoveChatBotEndpoint(
        IExternalChatsReadableRepository repository,
        IServiceScopeFactory scopeFactory,
        Serilog.ILogger logger
    )
    {
        _logger = logger;
        _repository = repository;
        _scopeFactory = scopeFactory;
    }

    /// <summary>
    /// Точка входа для удаления чата (или темы чата)
    /// </summary>
    /// <param name="client">Telegram bot клиент для общения с Telegram</param>
    /// <param name="update">Последнее событие</param>
    [ReplyMenuHandler(CommandComparison.Contains, "/remove_this_chat@", "/remove_this_chat")]
    public async Task Handle(ITelegramBotClient client, Update update)
    {
        _logger.Information("{Context} invoked", Context);
        Result<TelegramBotUser> user = update.GetUser();
        if (user.IsFailure)
            return;

        long userId = user.Value.Id;
        Result<int> themeId = update.GetThemeId();
        long chatId = update.GetChatId();

        // последний чат пользователя или нет. Если чат последний - запись пользователя будет удалена из БД.
        // здесь имеется ввиду последний ОСНОВНОЙ чат (не тема).
        // поскольку пользователь без чата бесполезен в системе
        // чтобы пользователь "жил" в системе, ему нужен хотя бы 1 чат.
        bool isLastChat = await _repository.IsLastUserChat(userId);

        Result result = await HandleMethod(userId, chatId, themeId, isLastChat);
        if (result.IsFailure)
        {
            await result.SendError(client, update);
            return;
        }

        await SendReplyMessage(client, chatId, themeId);
    }

    /// <summary>
    /// Обработчик удаления чата
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="chatId">ID чата</param>
    /// <param name="themeResult">ID темы Success или Failure. При Failure удаление будет происходить для основого чата</param>
    /// <param name="isLastChat">Последний чат пользователя или нет</param>
    /// <returns>Success или Failure</returns>
    private Task<Result> HandleMethod(
        long userId,
        long chatId,
        Result<int> themeResult,
        bool isLastChat
    ) =>
        themeResult.IsSuccess
            ? HandleForThemeChat(userId, chatId, themeResult.Value)
            : HandleForChat(userId, chatId, isLastChat);

    /// <summary>
    /// Обработка удаления основного чата пользователя
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="chatId">ID чата</param>
    /// <param name="isLastChat">Последний чат пользователя или нет</param>
    /// <returns>Success или Failure</returns>
    private async Task<Result> HandleForChat(long userId, long chatId, bool isLastChat)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var facade = scope.ServiceProvider.GetRequiredService<RemoveOwnerChatFacade>();
        return await facade.RemoveOwnerChat(userId, chatId, isLastChat);
    }

    /// <summary>
    /// Обработка удаления темы чата пользователя
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="chatId">ID чата</param>
    /// <param name="themeId">ID темы чата</param>
    /// <returns>Success или Failure</returns>
    private async Task<Result> HandleForThemeChat(long userId, long chatId, long themeId)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var facade = scope.ServiceProvider.GetRequiredService<RemoveThemeChatFacade>();
        return await facade.RemoveThemeChat(userId, chatId, themeId);
    }

    /// <summary>
    /// Отправка ответного сообщения, что был удален чат/тема чата.
    /// </summary>
    /// <param name="client">Telegram bot клиент для общения с Telegram</param>
    /// <param name="chatId">ID чата</param>
    /// <param name="themeResult">ID темы чата</param>
    /// <returns></returns>
    private static Task SendReplyMessage(
        ITelegramBotClient client,
        long chatId,
        Result<int> themeResult
    ) =>
        themeResult.IsSuccess
            ? client.SendMessage(
                chatId: chatId,
                text: "Тема отписана",
                messageThreadId: themeResult.Value
            )
            : client.SendMessage(chatId: chatId, text: "Чат отписан");
}
