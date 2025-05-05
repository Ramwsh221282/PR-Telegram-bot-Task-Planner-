using CSharpFunctionalExtensions;
using PRTelegramBot.Attributes;
using PRTelegramBot.Extensions;
using PRTelegramBot.Models.Enums;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChat;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChatOwner;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;
using RocketTaskPlanner.Application.NotificationsContext.Features.RemoveChat;
using RocketTaskPlanner.Application.NotificationsContext.Features.RemoveTheme;
using RocketTaskPlanner.Application.NotificationsContext.Visitor;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.RemoveChatEndpoint;

[BotHandler]
public sealed class RemoveChatBotEndpoint
{
    private readonly INotificationUseCaseVisitor _notificationsVisitor;
    private readonly IExternalChatUseCasesVisitor _chatsVisitor;
    private readonly IExternalChatsReadableRepository _repository;

    public RemoveChatBotEndpoint(
        INotificationUseCaseVisitor notificationsVisitor,
        IExternalChatUseCasesVisitor chatsVisitor,
        IExternalChatsReadableRepository repository
    )
    {
        _notificationsVisitor = notificationsVisitor;
        _chatsVisitor = chatsVisitor;
        _repository = repository;
    }

    [ReplyMenuHandler(CommandComparison.Equals, "/remove")]
    public async Task Handle(ITelegramBotClient client, Update update)
    {
        Result<TelegramBotUser> invokeUser = update.GetUser();
        if (invokeUser.IsFailure)
        {
            await invokeUser.SendError(client, update);
            return;
        }

        long chatId = update.GetChatId();
        Result<int> themeId = update.GetThemeId();

        Result<ExternalChatOwner> owner = await GetUserAsChatOwner(invokeUser.Value);
        if (owner.IsFailure)
        {
            await SendNoPermissionsError(client, chatId, themeId);
            return;
        }

        Task handle = themeId.IsSuccess switch
        {
            true => HandleForTheme(chatId, themeId.Value, owner.Value, client, update),
            false => HandleForGeneralChat(chatId, owner.Value, client, update),
        };

        await handle;
    }

    private async Task<Result<ExternalChatOwner>> GetUserAsChatOwner(TelegramBotUser user)
    {
        Result<ExternalChatOwner> owner = await _repository.GetExternalChatOwnerById(user.Id);
        return owner;
    }

    private async Task HandleForTheme(
        long chatId,
        int themeId,
        ExternalChatOwner owner,
        ITelegramBotClient client,
        Update update
    )
    {
        if (!owner.OwnsChat(themeId))
        {
            await SendNoPermissionsError(client, themeId, chatId);
            return;
        }

        RemoveThemeUseCase removeTheme = new(chatId, themeId);
        RemoveExternalChatUseCase removeExternalChat = new(owner.Id.Value, themeId);
        Result removingTheme = await _notificationsVisitor.Visit(removeTheme);

        if (removingTheme.IsFailure)
        {
            await removingTheme.SendError(client, update);
            return;
        }

        Result removingChat = await _chatsVisitor.Visit(removeExternalChat);
        if (removingChat.IsFailure)
        {
            await removingChat.SendError(client, update);
            return;
        }

        await client.SendMessage(chatId: chatId, text: "Тема отписана", messageThreadId: themeId);
    }

    private async Task HandleForGeneralChat(
        long chatId,
        ExternalChatOwner owner,
        ITelegramBotClient client,
        Update update
    )
    {
        if (!owner.OwnsChat(chatId))
        {
            await SendNoPermissionsError(client, chatId);
            return;
        }

        RemoveChatUseCase removeChat = new(chatId);
        RemoveExternalChatUseCase removeExternalChat = new(owner.Id.Value, chatId);
        Result removingChat = await _notificationsVisitor.Visit(removeChat);
        if (removingChat.IsFailure)
        {
            await removingChat.SendError(client, update);
            return;
        }

        Result removingChatOwner = await _chatsVisitor.Visit(removeExternalChat);
        if (removingChatOwner.IsFailure)
        {
            await removingChatOwner.SendError(client, update);
            return;
        }

        await client.SendMessage(chatId: chatId, text: "Чат отписан");

        // удаление пользователя как обладателя чата, если удаленный чат был последним.
        ExternalChatOwner updatedOwnerState = (
            await _repository.GetExternalChatOwnerById(owner.Id.Value)
        ).Value;
        if (updatedOwnerState.Chats.Count == 0)
        {
            RemoveExternalChatOwnerUseCase removeOwnwer = new(updatedOwnerState.Id.Value);
            await _chatsVisitor.Visit(removeOwnwer);
        }
    }

    private static async Task SendNoPermissionsError(
        ITelegramBotClient client,
        long chatId,
        Result<int> themeResult
    )
    {
        string errorMessage = "Операция доступна тому, кто добавлял этот чат.";
        Task operation = themeResult.IsSuccess switch
        {
            true => client.SendMessage(
                chatId: chatId,
                text: errorMessage,
                messageThreadId: themeResult.Value
            ),
            false => client.SendMessage(chatId: chatId, text: errorMessage),
        };
        await operation;
    }

    private static async Task SendNoPermissionsError(ITelegramBotClient client, long chatId)
    {
        string errorMessage = "Операция доступна тому, кто добавлял этот чат.";
        await client.SendMessage(chatId: chatId, text: errorMessage);
    }

    private static async Task SendNoPermissionsError(
        ITelegramBotClient client,
        int themeId,
        long chatId
    )
    {
        string errorMessage = "Операция доступна тому, кто добавлял этот чат.";
        await client.SendMessage(chatId: chatId, text: errorMessage, messageThreadId: themeId);
    }
}
