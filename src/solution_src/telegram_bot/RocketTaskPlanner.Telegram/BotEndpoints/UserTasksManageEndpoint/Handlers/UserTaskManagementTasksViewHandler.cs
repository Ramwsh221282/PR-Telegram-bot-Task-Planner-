using System.Text.RegularExpressions;
using PRTelegramBot.Models;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetPageCountForChatSubjects;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetPagedChatSubjects;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Cache;
using RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Constants;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Handlers;

/// <summary>
/// Меню списка задач.
/// Содержит логику нажатия на задачу и нажатия на кнопку "назад в выбор типа задач"
/// </summary>
public sealed partial class UserTaskManagementTasksViewHandler : ITelegramBotHandler
{
    /// <summary>
    /// <inheritdoc cref="TelegramBotExecutionContext"/>
    /// </summary>
    private readonly TelegramBotExecutionContext _context;

    /// <summary>
    /// <inheritdoc cref="IQueryHandler{TQuery,TQueryResponse}"/>
    /// </summary>
    private readonly IQueryHandler<
        GetPagedChatSubjectsQuery,
        GetPagedChatSubjectsQueryResponse
    > _itemsHandler;

    /// <summary>
    /// <inheritdoc cref="IQueryHandler{TQuery,TQueryResponse}"/>
    /// </summary>
    private readonly IQueryHandler<GetPageCountForChatSubjectsQuery, int> _pagesCountHandler;

    /// <summary>
    /// <inheritdoc cref="ITelegramBotHandler.Command"/>
    /// </summary>
    public string Command => HandlerNames.TasksViewHandler;

    public UserTaskManagementTasksViewHandler(
        TelegramBotExecutionContext context,
        IQueryHandler<GetPagedChatSubjectsQuery, GetPagedChatSubjectsQueryResponse> itemsHandler,
        IQueryHandler<GetPageCountForChatSubjectsQuery, int> pagesCountHandler
    )
    {
        _context = context;
        _itemsHandler = itemsHandler;
        _pagesCountHandler = pagesCountHandler;
    }

    /// <summary>
    /// <inheritdoc cref="ITelegramBotHandler.Handle"/>
    /// </summary>
    public async Task Handle(ITelegramBotClient client, Update update)
    {
        var message = update.GetMessage();
        if (message.IsFailure)
            return;

        var cache = _context.GetCacheInfo<UserTaskManagementCache>();
        if (cache.IsFailure)
            return;

        var cacheValue = cache.Value;

        // если была нажата кнопка назад в типы задач - возврат в меню типа задач.
        if (message.Value == TaskManageConstants.BackToTaskTypes)
        {
            await BackToTaskTypeMenu(client, update, cacheValue);
            return;
        }

        // обновление данных кеша, если была изменена страница.
        cacheValue = UpdateCacheIfPageChanged(message.Value, cacheValue);

        // обновление данных кеша (списка задач)
        cacheValue = await GetChatSubjects(cacheValue);

        // обновление пагинации
        cacheValue = await GetPagesCount(cacheValue);

        // если выбрана задача, то перейти в меню конкретной задачи.
        if (IsTaskSelected(message.Value))
        {
            await client.ClearMenu(update, "Переход в меню задачи");
            cacheValue = SetSelectedTask(message.Value, cacheValue);
            await AssignConcreteTaskHandler(client, update, cacheValue);
        }
        // в любом другом случае (например следующая страница, повторный вызов обработчика меню списка задач.
        else
            await AssignTasksViewHandler(client, update, cacheValue);
    }

    /// <summary>
    /// Создать меню списка задач
    /// </summary>
    /// <param name="cache">Кеш</param>
    /// <returns>Меню списка задач</returns>
    private static ReplyKeyboardMarkup BuildTasksList(UserTaskManagementCache cache)
    {
        var menuButtons = new List<KeyboardButton[]>();
        var navButtons = new List<KeyboardButton>();

        // добавление кнопок задач
        foreach (var item in cache.CurrentTasks)
        {
            long id = item.GetSubjectId();
            string text = item.GetSubjectMessage();
            string buttonText = $"{id} {text}";
            menuButtons.Add([new KeyboardButton(buttonText)]);
        }

        // добавление навигационных кнопок (пагинация + кнопка назад)
        navButtons.Add(new KeyboardButton(TaskManageConstants.BackToTaskTypes));
        for (int i = 1; i <= cache.PagesCount; i++)
            navButtons.Add(new KeyboardButton(i.ToString()));

        menuButtons.Add(navButtons.ToArray());
        return new ReplyKeyboardMarkup(menuButtons) { ResizeKeyboard = true };
    }

    /// <summary>
    /// Создание меню конкретной задачи
    /// </summary>
    /// <param name="cache">Кеш</param>
    /// <returns>Меню</returns>
    private static ReplyKeyboardMarkup BuildConcreteTaskMenu(UserTaskManagementCache cache)
    {
        var menuButtons = new List<KeyboardButton[]>();
        var controlButtons = new List<KeyboardButton>();

        // добавлении кнопки, которая содержит информацию о задаче (визуал)
        var subject = cache.GetSelectedSubject();
        var subjectMessage = subject.GetSubjectMessage();
        menuButtons.Add([new KeyboardButton(subjectMessage)]);

        // добавление кнопок навигации и управления (назад и удалить задачу)
        controlButtons.Add(new KeyboardButton(TaskManageConstants.BackToList));
        controlButtons.Add(new KeyboardButton(TaskManageConstants.DeleteTask));
        menuButtons.Add(controlButtons.ToArray());

        return new ReplyKeyboardMarkup(menuButtons) { ResizeKeyboard = true };
    }

    /// <summary>
    /// Получить задачи
    /// </summary>
    /// <param name="cache">Кеш</param>
    /// <returns>Обновленный кеш</returns>
    private async Task<UserTaskManagementCache> GetChatSubjects(UserTaskManagementCache cache)
    {
        long chatId = cache.GetSelectedChatId();
        int page = cache.CurrentPage;
        int pageSize = cache.PageSize;
        bool isPeriodic = cache.IsPeriodicView;

        GetPagedChatSubjectsQuery query = new(chatId, page, pageSize, isPeriodic);
        GetPagedChatSubjectsQueryResponse response = await _itemsHandler.Handle(query);
        var cacheWithItems = cache with { CurrentTasks = response.Subjects };
        return cacheWithItems;
    }

    /// <summary>
    /// Получить пагинацию
    /// </summary>
    /// <param name="cache">Кеш</param>
    /// <returns>Обновленный кеш</returns>
    private async Task<UserTaskManagementCache> GetPagesCount(UserTaskManagementCache cache)
    {
        if (cache.PagesCount != -1)
            return cache;

        long chatId = cache.GetSelectedChatId();
        int pageSize = cache.PageSize;
        bool isPeriodic = cache.IsPeriodicView;

        var query = new GetPageCountForChatSubjectsQuery(chatId, pageSize, isPeriodic);
        var count = await _pagesCountHandler.Handle(query);
        return cache with { PagesCount = count };
    }

    /// <summary>
    /// Обновление данных кеша если изменена текущая страница
    /// </summary>
    /// <param name="message">Сообщение пользователя</param>
    /// <param name="cache">Кеш</param>
    /// <returns>Обновленный кеш</returns>
    private static UserTaskManagementCache UpdateCacheIfPageChanged(
        string message,
        UserTaskManagementCache cache
    )
    {
        // если сообщение не является числом (то есть если пользователь не тыкнул на страницу в меню)
        // ничего не делать
        if (!message.All(char.IsDigit))
            return cache;

        int updatedPage = int.Parse(message);
        return cache with { CurrentPage = updatedPage };
    }

    /// <summary>
    /// Проверка Regex'ом, что была выбрана кнопка задачи
    /// </summary>
    /// <param name="message">Сообщение пользователя</param>
    /// <returns>True если задача, False в другом случае</returns>
    private static bool IsTaskSelected(string message)
    {
        // проверяем, что сообщение соответствует regex шаблону
        var match = TaskSelectedDetectRegex().Match(message);
        if (match.Success == false)
            return false;

        // вернёт True если обнаружено 3 группы при матчинге Regex.
        // Группа 1 - строка сообщения пользователя целиком
        // Группа 2 - часть с ID задачи
        // Группа 3 - часть с сообщением задачи
        // ( (-123213132321) (Завтра сделать зарядку в 9:00) ) - визуальное представление групп.
        return match.Groups.Count == 3;
    }

    /// <summary>
    /// Установить в кеш выбранную задачу.
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="cache">Кеш</param>
    /// <returns>Обновленный кеш</returns>
    /// <exception cref="ApplicationException">Исключение, если не соответствует Regex</exception>
    private static UserTaskManagementCache SetSelectedTask(
        string message,
        UserTaskManagementCache cache
    )
    {
        // проверяем, что соответствует Regex
        var match = TaskSelectedDetectRegex().Match(message);
        if (match.Success == false)
            throw new ApplicationException();

        // берем часть с ID
        var subjectId = long.Parse(match.Groups[1].Value);

        // получаем сообщение из кеша по ID
        var subject = cache.GetSubjectById(subjectId);

        // возвращаем обновленный кеш
        return cache.SetSelectedSubject(subject);
    }

    /// <summary>
    /// Назначить обработчика меню конкретной задачи
    /// </summary>
    /// <param name="client">ITelegram bot клиент для общения с Telegram</param>
    /// <param name="update">Последнее событие</param>
    /// <param name="cache">Кеш</param>
    private async Task AssignConcreteTaskHandler(
        ITelegramBotClient client,
        Update update,
        UserTaskManagementCache cache
    )
    {
        var menu = BuildConcreteTaskMenu(cache);
        var options = new OptionMessage() { MenuReplyKeyboardMarkup = menu };

        await PRTelegramBot.Helpers.Message.Send(
            client,
            update,
            $"Выбрана задача: {cache.GetSelectedSubject().GetSubjectMessage()}",
            options
        );

        const string nextHandlerName = HandlerNames.ConcreteTaskViewHandler;
        var nextHandler = _context.GetRequiredHandler(nextHandlerName);
        _context.AssignNextStep(update, nextHandler, cache);
    }

    /// <summary>
    /// Назначение обработчика выбора типа задач при нажатии на кнопку "Назад"
    /// </summary>
    /// <param name="client">ITelegram bot клиент для общения с Telegram</param>
    /// <param name="update">Последнее событие</param>
    /// <param name="cache">Кеш</param>
    private async Task BackToTaskTypeMenu(
        ITelegramBotClient client,
        Update update,
        UserTaskManagementCache cache
    )
    {
        cache = cache with { PagesCount = -1, CurrentPage = 1, CurrentTasks = [] };
        var handlerName = HandlerNames.ChooseTaskTypesHandler;
        var handler = _context.GetRequiredHandler(handlerName);
        await _context.AssignAndRun(client, update, handler, cache);
    }

    /// <summary>
    /// Назначение обработчика меню списка задач для пагинационных кнопок.
    /// </summary>
    /// <param name="client">ITelegram bot клиент для общения с Telegram</param>
    /// <param name="update">Последнее событие</param>
    /// <param name="cache">Кеш</param>
    private async Task AssignTasksViewHandler(
        ITelegramBotClient client,
        Update update,
        UserTaskManagementCache cache
    )
    {
        var menu = BuildTasksList(cache);
        var options = new OptionMessage { MenuReplyKeyboardMarkup = menu };

        await PRTelegramBot.Helpers.Message.Send(
            client,
            update,
            $"Список задач страница: {cache.CurrentPage}",
            options
        );

        _context.AssignNextStep(update, this, cache);
    }

    /// <summary>
    /// Regex для проверки, что была выбрана задача.
    /// </summary>
    /// <returns>Regex</returns>
    [GeneratedRegex(@"(.\d+)\s(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex TaskSelectedDetectRegex();
}
