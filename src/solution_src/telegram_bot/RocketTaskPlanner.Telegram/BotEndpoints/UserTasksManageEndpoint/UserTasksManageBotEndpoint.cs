using PRTelegramBot.Attributes;
using PRTelegramBot.Models.Enums;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.NotificationsContext.Visitor;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetNotificationReceiversByIdArray;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetPageCountForChatSubjects;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetPagedChatSubjects;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Handlers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint;

[BotHandler]
public sealed class UserTasksManageBotEndpoint
{
    private readonly TelegramBotExecutionContext _context;

    public UserTasksManageBotEndpoint(
        IQueryHandler<
            GetNotificationReceiversByIdentifiersQuery,
            GetNotificationReceiversByIdentifiersQueryResponse[]
        > getChats,
        IQueryHandler<GetPagedChatSubjectsQuery, GetPagedChatSubjectsQueryResponse> getSubjects,
        IQueryHandler<GetPageCountForChatSubjectsQuery, int> pageCount,
        IExternalChatsReadableRepository repository,
        INotificationUseCaseVisitor useCases
    )
    {
        var context = new TelegramBotExecutionContext();

        var validation = new UserTaskManagementValidationHandler(context, repository, getChats);
        var tasksList = new UserTaskManagementTasksViewHandler(context, getSubjects, pageCount);
        var concreteTask = new UserTaskManagementConcreteTaskViewHandler(context, useCases);
        var chooseTaskType = new UserTaskManagementChooseTaskTypesHandler(context);
        var dispatchTaskType = new UserTaskManagementTypeDispatchHandler(context);
        var onChatChosen = new UserTaskManagementOnChatChosenHandler(context);
        var chooseChat = new UserTaskManagementChooseChatHandler(context);

        _context = context
            .SetEntryPointHandler(validation)
            .RegisterHandler(chooseChat)
            .RegisterHandler(onChatChosen)
            .RegisterHandler(chooseTaskType)
            .RegisterHandler(dispatchTaskType)
            .RegisterHandler(tasksList)
            .RegisterHandler(concreteTask);
    }

    [SlashHandler(CommandComparison.Equals, StringComparison.OrdinalIgnoreCase, ["/my_tasks"])]
    public async Task ManageTasksHandler(ITelegramBotClient client, Update update)
    {
        await _context.InvokeEntryPoint(client, update);
    }
}
