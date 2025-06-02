using PRTelegramBot.Extensions;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Entities;
using RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Queries.GetNotificationReceiversByIdArray;
using RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Queries.GetReceiverThemesByParentId;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Cache;
using RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Constants;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Handlers;

/// <summary>
/// Обработчик валидации при вызове команды /my_tasks
/// </summary>
public sealed class UserTaskManagementValidationHandler : ITelegramBotHandler
{
    /// <summary>
    /// <inheritdoc cref="TelegramBotExecutionContext"/>
    /// </summary>
    private readonly TelegramBotExecutionContext _context;

    /// <summary>
    /// <inheritdoc cref="IExternalChatsReadableRepository"/>
    /// </summary>
    private readonly IExternalChatsReadableRepository _repository;

    /// <summary>
    /// <inheritdoc cref="IQueryHandler{TQuery,TQueryResponse}"/>
    /// </summary>
    private readonly IQueryHandler<
        GetNotificationReceiversByIdentifiersQuery,
        GetNotificationReceiversByIdentifiersQueryResponse[]
    > _queryHandler;

    private readonly IQueryHandler<GetNotificationReceiverThemesByParentId, ReceiverThemeEntity[]> _getThemes;

    public UserTaskManagementValidationHandler(
        TelegramBotExecutionContext context,
        IExternalChatsReadableRepository repository,
        IQueryHandler<
            GetNotificationReceiversByIdentifiersQuery,
            GetNotificationReceiversByIdentifiersQueryResponse[]
        > queryHandler,
        IQueryHandler<GetNotificationReceiverThemesByParentId, ReceiverThemeEntity[]> getThemes
    )
    {
        _context = context;
        _repository = repository;
        _queryHandler = queryHandler;
        _getThemes = getThemes;
    }

    /// <summary>
    /// <inheritdoc cref="ITelegramBotHandler.Command"/>
    /// </summary>
    public string Command => HandlerNames.ValidationHandler;

    /// <summary>
    /// <inheritdoc cref="ITelegramBotHandler.Handle"/>
    /// </summary>
    public async Task Handle(ITelegramBotClient client, Update update)
    {
        var user = update.GetUser();
        if (user.IsFailure)
            return;

        var owner = await _repository.GetExternalChatOwnerById(user.Value.Id);
        if (owner.IsFailure)
        {
            const string message = "У пользователя ещё нет чатов.";
            await client.SendMessage(chatId: update.GetChatId(), text: message);
            return;
        }

        var parentChats = owner.Value.GetGeneralChats();
        long[] parentChatIdentifiers = [.. parentChats.Select(p => p.Id.Value)];
        
        var query = new GetNotificationReceiversByIdentifiersQuery(parentChatIdentifiers);
        var response = await _queryHandler.Handle(query);
        List<UserTaskManagementChat> chats = [];
        foreach (var parent in response)
        {
            var generalChat = new UserTaskManagementGeneralChat(parent.ChatName, parent.ChatId);
            chats.Add(generalChat);
            var themesQuery = new GetNotificationReceiverThemesByParentId(parent.ChatId);
            var themes = await _getThemes.Handle(themesQuery);
            foreach (var theme in themes)
                chats.Add(new UserTaskManagementThemeChat(generalChat, theme.ThemeId));
        }
        
        var cache = new UserTaskManagementCache(chats.ToArray());

        const string nextHandlerName = HandlerNames.ChooseChatHandler;
        var handler = _context.GetRequiredHandler(nextHandlerName);

        // назначение следующего обработчика - меню выбора чатов.
        await _context.AssignAndRun(client, update, handler, cache);
    }
}
