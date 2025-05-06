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

[BotHandler]
public sealed class RemoveChatBotEndpoint
{
    // Хранилище пользователей и пользовательских чатов
    private readonly IExternalChatsReadableRepository _repository;

    // Фасадный класс для транзакции удаления пользовательского чата и чата уведомлений.
    // Если при удалении, удаляется пользователь у которого был 1 чат.
    // Пользователь будет так же удален
    private readonly RemoveOwnerChatFacade _removeOwnerChat;

    // Фасадный класс для транзакции удаления темы пользовательского чата и дочернего чата.
    private readonly RemoveThemeChatFacade _removeThemeChat;

    public RemoveChatBotEndpoint(
        IExternalChatsReadableRepository repository,
        RemoveOwnerChatFacade removeOwnerChat,
        RemoveThemeChatFacade removeThemeChat
    )
    {
        _repository = repository;
        _removeOwnerChat = removeOwnerChat;
        _removeThemeChat = removeThemeChat;
    }

    [ReplyMenuHandler(
        CommandComparison.Equals,
        StringComparison.OrdinalIgnoreCase,
        "/remove_this_chat"
    )]
    public async Task Handle(ITelegramBotClient client, Update update)
    {
        Result<TelegramBotUser> user = update.GetUser();
        if (user.IsFailure)
            return;

        long userId = user.Value.Id;
        Result<int> themeId = update.GetThemeId();
        long chatId = update.GetChatId();
        bool isLastChat = await _repository.IsLastUserChat(userId);

        Result result = await HandleMethod(userId, chatId, themeId, isLastChat);
        if (result.IsFailure)
        {
            await result.SendError(client, update);
            return;
        }

        await SendReplyMessage(client, chatId, themeId);
    }

    public Task<Result> HandleMethod(
        long userId,
        long chatId,
        Result<int> themeResult,
        bool isLastChat
    ) =>
        themeResult.IsSuccess
            ? HandleForThemeChat(userId, chatId, themeResult.Value)
            : HandleForChat(userId, chatId, isLastChat);

    public async Task<Result> HandleForChat(long userId, long chatId, bool isLastChat) =>
        await _removeOwnerChat.RemoveOwnerChat(userId, chatId, isLastChat);

    public async Task<Result> HandleForThemeChat(long userId, long chatId, long themeId) =>
        await _removeThemeChat.RemoveThemeChat(userId, chatId, themeId);

    public Task SendReplyMessage(ITelegramBotClient client, long chatId, Result<int> themeResult) =>
        themeResult.IsSuccess
            ? client.SendMessage(
                chatId: chatId,
                text: "Тема отписана",
                messageThreadId: themeResult.Value
            )
            : client.SendMessage(chatId: chatId, text: "Чат отписан");
}
