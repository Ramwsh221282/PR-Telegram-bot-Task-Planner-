using PRTelegramBot.Extensions;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetNotificationReceiversByIdArray;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Cache;
using RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Constants;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Handlers;

public sealed class UserTaskManagementValidationHandler : ITelegramBotHandler
{
    private readonly TelegramBotExecutionContext _context;
    private readonly IExternalChatsReadableRepository _repository;
    private readonly IQueryHandler<
        GetNotificationReceiversByIdentifiersQuery,
        GetNotificationReceiversByIdentifiersQueryResponse[]
    > _queryHandler;

    public UserTaskManagementValidationHandler(
        TelegramBotExecutionContext context,
        IExternalChatsReadableRepository repository,
        IQueryHandler<
            GetNotificationReceiversByIdentifiersQuery,
            GetNotificationReceiversByIdentifiersQueryResponse[]
        > queryHandler
    )
    {
        _context = context;
        _repository = repository;
        _queryHandler = queryHandler;
    }

    public string Command => HandlerNames.ValidationHandler;

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
        var cache = new UserTaskManagementCache(response);

        const string nextHandlerName = HandlerNames.ChooseChatHandler;
        var handler = _context.GetRequiredHandler(nextHandlerName);
        await _context.AssignAndRun(client, update, handler, cache);
    }
}
