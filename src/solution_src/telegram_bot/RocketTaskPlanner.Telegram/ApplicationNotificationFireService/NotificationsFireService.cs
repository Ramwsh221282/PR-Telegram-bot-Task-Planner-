using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Cache;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Receivers;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks.GeneralChatTasks;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks.ThemeChatTasks;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Times;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Times.Decorators.Times;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Times.Decorators.TimesList;
using RocketTaskPlanner.Telegram.TelegramBotFactories;
using Telegram.Bot;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService;

/// <summary>
/// Background процесс для отправки уведомлений
/// </summary>
/// <param name="timeCache">Кеш временных зон</param>
/// <param name="logger">Логгер</param>
/// <param name="connectionFactory">Фабрика для создания соединения с БД</param>
/// <param name="botFactory">Фабрика для создания экземпляра бота</param>
public sealed class NotificationsFireService(
    TimeZoneDbProviderCachedInstance timeCache,
    Serilog.ILogger logger,
    IDbConnectionFactory connectionFactory,
    TelegramBotClientFactory botFactory
) : BackgroundService
{
    private readonly TimeZoneDbProviderCachedInstance _timeCache = timeCache;
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;
    private readonly TelegramBotClientFactory _botFactory = botFactory;
    private readonly Serilog.ILogger _logger = logger;
    private const string CONTEXT = nameof(NotificationsFireService);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            ITimeZoneOfCurrentTime[] times = Times(); // Получение списка временных зон.
            GeneralChatReceiverOfCurrentTimeZone[] receivers = await Receivers(times); // Получение списка получателей на основе временных зон.
            IThemeChatTaskToFire[] themeChatTasksToFire = ThemeChatTasks(receivers); // Получение списка тем, куда нужно отправить сообщения.
            IGeneralChatTaskToFire[] generalChatTasksToFire = GeneralChatTasks(receivers); // Получение списка чатов, куда нужно отправить сообщения.
            int fired = await HandleTasksToFire(themeChatTasksToFire, generalChatTasksToFire); // Отправка сообщений.
            _logger.Information("{Context} fired: {Count} tasks", CONTEXT, fired);
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private ITimeZoneOfCurrentTime[] Times()
    {
        TimeZoneOfCurrentTimeList list = new([]);
        TimeZoneOfCurrentTimeListCached cached = new(list, _timeCache);
        TimeZoneOfCurrentTimeListLoggable log = new(cached, _logger);

        return [.. log.Select(t => new SqlSpeakingTimeZoneOfCurrentTime(t, _connectionFactory))];
    }

    private static async Task<GeneralChatReceiverOfCurrentTimeZone[]> Receivers(
        ITimeZoneOfCurrentTime[] receivers
    )
    {
        List<GeneralChatReceiverOfCurrentTimeZone> receiversList = [];
        foreach (ITimeZoneOfCurrentTime time in receivers)
            receiversList.AddRange(await time.Receivers());

        return [.. receiversList];
    }

    private IThemeChatTaskToFire[] ThemeChatTasks(GeneralChatReceiverOfCurrentTimeZone[] receivers)
    {
        IThemeChatTaskToFire[] tasks = [.. receivers.SelectMany(r => r.ThemeChatUnfiredTasks())];
        TelegramBotClient bot = _botFactory.Create();
        for (int index = 0; index < tasks.Length; index++)
        {
            IThemeChatTaskToFire target = tasks[index];
            ThemeChatTaskToFireSqlSpeaking sql = new(target, _connectionFactory);
            ThemeChatTaskToFireTelegramSpeaking telegram = new(sql, bot);
            ThemeChatTaskToFireLog log = new(telegram, _logger);
            tasks[index] = log;
        }

        return tasks;
    }

    private IGeneralChatTaskToFire[] GeneralChatTasks(
        GeneralChatReceiverOfCurrentTimeZone[] receivers
    )
    {
        IGeneralChatTaskToFire[] tasks = [.. receivers.SelectMany(s => s.TasksToFire())];
        TelegramBotClient bot = _botFactory.Create();
        for (int index = 0; index < tasks.Length; index++)
        {
            IGeneralChatTaskToFire target = tasks[index];
            GeneralChatTaskToFireSqlSpeaking sql = new(target, _connectionFactory);
            GeneralChatTaskToFireTelegramSpeaking telegram = new(sql, bot);
            GeneralChatTaskToFireLog log = new(telegram, _logger);
            tasks[index] = log;
        }

        return tasks;
    }

    private async Task<int> HandleTasksToFire(
        IThemeChatTaskToFire[] themeChatTasks,
        IGeneralChatTaskToFire[] generalChatTasks
    )
    {
        List<TaskFromRemovedChat> unfired = [];
        int count = 0;

        foreach (IGeneralChatTaskToFire task in generalChatTasks)
        {
            ITaskToFire processed = await task.Fire();
            if (processed is TaskFromRemovedChat removed)
                unfired.Add(removed);
            count++;
        }

        foreach (IThemeChatTaskToFire task in themeChatTasks)
        {
            ITaskToFire processed = await task.Fire();
            if (processed is TaskFromRemovedChat removed)
                unfired.Add(removed);
            count++;
        }

        foreach (TaskFromRemovedChat removed in unfired)
            await removed.HandleTaskFromRemovedChat(_connectionFactory);

        return count;
    }
}
