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

public sealed partial class UserTaskManagementTasksViewHandler : ITelegramBotHandler
{
    private readonly TelegramBotExecutionContext _context;
    private readonly IQueryHandler<
        GetPagedChatSubjectsQuery,
        GetPagedChatSubjectsQueryResponse
    > _itemsHandler;

    private readonly IQueryHandler<GetPageCountForChatSubjectsQuery, int> _pagesCountHandler;

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

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        var message = update.GetMessage();
        if (message.IsFailure)
            return;

        var cache = _context.GetCacheInfo<UserTaskManagementCache>();
        if (cache.IsFailure)
            return;

        var cacheValue = cache.Value;

        if (message.Value == TaskManageConstants.BackToTaskTypes)
        {
            await BackToTaskTypeMenu(client, update, cacheValue);
            return;
        }

        cacheValue = UpdateCacheIfPageChanged(message.Value, cacheValue);
        cacheValue = await GetChatSubjects(cacheValue);
        cacheValue = await GetPagesCount(cacheValue);

        if (IsTaskSelected(message.Value))
        {
            await client.ClearMenu(update, "Переход в меню задачи");
            cacheValue = SetSelectedTask(message.Value, cacheValue);
            await AssignConcreteTaskHandler(client, update, cacheValue);
        }
        else
            await AssignTasksViewHandler(client, update, cacheValue);
    }

    private static ReplyKeyboardMarkup BuildTasksList(UserTaskManagementCache cache)
    {
        var menuButtons = new List<KeyboardButton[]>();
        var navButtons = new List<KeyboardButton>();

        foreach (var item in cache.CurrentTasks)
        {
            long id = item.GetSubjectId();
            string text = item.GetSubjectMessage();
            string buttonText = $"{id} {text}";
            menuButtons.Add([new KeyboardButton(buttonText)]);
        }

        navButtons.Add(new KeyboardButton(TaskManageConstants.BackToTaskTypes));
        for (int i = 1; i <= cache.PagesCount; i++)
            navButtons.Add(new KeyboardButton(i.ToString()));

        menuButtons.Add(navButtons.ToArray());
        return new ReplyKeyboardMarkup(menuButtons) { ResizeKeyboard = true };
    }

    private static ReplyKeyboardMarkup BuildConcreteTaskMenu(UserTaskManagementCache cache)
    {
        var menuButtons = new List<KeyboardButton[]>();
        var controlButtons = new List<KeyboardButton>();

        var subject = cache.GetSelectedSubject();
        var subjectMessage = subject.GetSubjectMessage();
        menuButtons.Add([new KeyboardButton(subjectMessage)]);

        controlButtons.Add(new KeyboardButton(TaskManageConstants.BackToList));
        controlButtons.Add(new KeyboardButton(TaskManageConstants.DeleteTask));
        menuButtons.Add(controlButtons.ToArray());

        return new ReplyKeyboardMarkup(menuButtons) { ResizeKeyboard = true };
    }

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

    private static UserTaskManagementCache UpdateCacheIfPageChanged(
        string message,
        UserTaskManagementCache cache
    )
    {
        if (!message.All(char.IsDigit))
            return cache;

        int updatedPage = int.Parse(message);
        return cache with { CurrentPage = updatedPage };
    }

    private static bool IsTaskSelected(string message)
    {
        var match = TaskSelectedDetectRegex().Match(message);
        if (match.Success == false)
            return false;

        return match.Groups.Count == 3;
    }

    private static UserTaskManagementCache SetSelectedTask(
        string message,
        UserTaskManagementCache cache
    )
    {
        var match = TaskSelectedDetectRegex().Match(message);
        if (match.Success == false)
            throw new ApplicationException();

        var subjectId = long.Parse(match.Groups[1].Value);
        var subject = cache.GetSubjectById(subjectId);
        return cache.SetSelectedSubject(subject);
    }

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

    [GeneratedRegex(@"(.\d+)\s(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex TaskSelectedDetectRegex();
}
