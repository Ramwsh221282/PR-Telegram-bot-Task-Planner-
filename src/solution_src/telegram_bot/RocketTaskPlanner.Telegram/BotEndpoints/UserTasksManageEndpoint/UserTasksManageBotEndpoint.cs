using PRTelegramBot.Attributes;
using PRTelegramBot.Models.Enums;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.NotificationsContext.Visitor;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Entities;
using RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Queries.GetNotificationReceiversByIdArray;
using RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Queries.GetPageCountForChatSubjects;
using RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Queries.GetPagedChatSubjects;
using RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Queries.GetReceiverThemesByParentId;
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
    private const string Context = "Endpoint. список задач пользователя";

    /// <summary>
    /// <inheritdoc cref="TelegramBotExecutionContext"/>
    /// </summary>
    private readonly TelegramBotExecutionContext _context;

    private readonly Serilog.ILogger _logger;

    /// <summary>
    /// Конструктор Endpoint
    /// </summary>
    /// <param name="getChats">Обработчик запроса на получение чатов (без тем).</param>
    /// <param name="getSubjects">Обработчик запроса на получение задач (включая задач тем).</param>
    /// <param name="pageCount">Обработчик запроса для получения пагинации.</param>
    /// <param name="repository">Хранилище внешних пользовательских чатов.</param>
    public UserTasksManageBotEndpoint(
        IQueryHandler<
            GetNotificationReceiversByIdentifiersQuery,
            GetNotificationReceiversByIdentifiersQueryResponse[]
        > getChats,
        IQueryHandler<GetPagedChatSubjectsQuery, GetPagedChatSubjectsQueryResponse> getSubjects,
        IQueryHandler<GetPageCountForChatSubjectsQuery, int> pageCount,
        IQueryHandler<GetNotificationReceiverThemesByParentId, ReceiverThemeEntity[]> getThemes,
        IExternalChatsReadableRepository repository,
        IServiceScopeFactory scopeFactory,
        Serilog.ILogger logger
    )
    {
        _logger = logger;

        // инициализация пустого контекста
        var context = new TelegramBotExecutionContext();

        // обработчик валидации (что пользователь - обладатель чатов), нужен для авторизации
        var validation = new UserTaskManagementValidationHandler(
            context,
            repository,
            getChats,
            getThemes
        );

        // обработчик для отображения списка задач в меню
        var tasksList = new UserTaskManagementTasksViewHandler(context, getSubjects, pageCount);

        // обработчик для отображения меню конкретной задачи (выбранной задачи)
        var concreteTask = new UserTaskManagementConcreteTaskViewHandler(context, scopeFactory);

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
    [ReplyMenuHandler(CommandComparison.Contains, "/my_tasks", "/my_tasks@")]
    public async Task ManageTasksHandler(ITelegramBotClient client, Update update)
    {
        _logger.Information("{Context} вызван.", Context);
        await _context.InvokeEntryPoint(client, update);
    }
}
