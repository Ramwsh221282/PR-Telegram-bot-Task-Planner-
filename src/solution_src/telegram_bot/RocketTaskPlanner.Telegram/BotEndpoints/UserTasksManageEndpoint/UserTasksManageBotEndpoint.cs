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

/// <summary>
/// Endpoint для работы со списком уведомлений в чатах.
/// Данный endpoint работает, если его вызывать в приват чате с ботом,
/// потому что используется меню Reply Keyboard Markup, которое работает только в приват чате бота.
/// </summary>
[BotHandler]
public sealed class UserTasksManageBotEndpoint
{
    /// <summary>
    /// <inheritdoc cref="TelegramBotExecutionContext"/>
    /// </summary>
    private readonly TelegramBotExecutionContext _context;

    /// <summary>
    /// Конструктор Endpoint
    /// </summary>
    /// <param name="getChats">Обработчик запроса на получение чатов (без тем).</param>
    /// <param name="getSubjects">Обработчик запроса на получение задач (включая задач тем).</param>
    /// <param name="pageCount">Обработчик запроса для получения пагинации.</param>
    /// <param name="repository">Хранилище внешних пользовательских чатов.</param>
    /// <param name="useCases">Обработчики бизнес логики чатов уведомлений.</param>
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
        // инициализация пустого контекста
        var context = new TelegramBotExecutionContext();

        // обработчик валидации (что пользователь - обладатель чатов), нужен для авторизации
        var validation = new UserTaskManagementValidationHandler(context, repository, getChats);

        // обработчик для отображения списка задач в меню
        var tasksList = new UserTaskManagementTasksViewHandler(context, getSubjects, pageCount);

        // обработчик для отображения меню конкретной задачи (выбранной задачи)
        var concreteTask = new UserTaskManagementConcreteTaskViewHandler(context, useCases);

        // обработчик для отображения меню выбора типа задач (период или без периода)
        var chooseTaskType = new UserTaskManagementChooseTaskTypesHandler(context);

        // обработчик выбора типа задач (когда выбирается тип задач пользователем)
        var dispatchTaskType = new UserTaskManagementTypeDispatchHandler(context);

        // обработчик выбора чатов (когда выбирается чат пользователем)
        var onChatChosen = new UserTaskManagementOnChatChosenHandler(context);

        // обработчик создания меню выбора чатов.
        var chooseChat = new UserTaskManagementChooseChatHandler(context);

        // инициализации контекста и регистрация обработчиков
        _context = context
            .SetEntryPointHandler(validation)
            .RegisterHandler(chooseChat)
            .RegisterHandler(onChatChosen)
            .RegisterHandler(chooseTaskType)
            .RegisterHandler(dispatchTaskType)
            .RegisterHandler(tasksList)
            .RegisterHandler(concreteTask);
    }

    /// <summary>
    /// Точка входа в endpoint обработки /my_tasks
    /// </summary>
    /// <param name="client">Telegram bot клиент для общений с телеграм</param>
    /// <param name="update">Последнее событие</param>
    [SlashHandler(CommandComparison.Equals, StringComparison.OrdinalIgnoreCase, ["/my_tasks"])]
    public async Task ManageTasksHandler(ITelegramBotClient client, Update update)
    {
        await _context.InvokeEntryPoint(client, update);
    }
}
