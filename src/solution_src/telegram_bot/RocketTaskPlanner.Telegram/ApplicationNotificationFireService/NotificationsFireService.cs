using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Database.ApplicationTimeContext.Cache;
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
/// <param name="timeCache">
///     <inheritdoc cref="TimeZoneDbProviderCachedInstance"/>
/// </param>
/// <param name="logger">
///     <inheritdoc cref="Serilog.ILogger"/>
/// </param>
/// <param name="connectionFactory">
///     <inheritdoc cref="IDbConnectionFactory"/>
/// </param>
/// <param name="botFactory">
///     <inheritdoc cref="TelegramBotClientFactory"/>
/// </param>
/// </summary>
public sealed class NotificationsFireService(
    TimeZoneDbProviderCachedInstance timeCache,
    Serilog.ILogger logger,
    IDbConnectionFactory connectionFactory,
    TelegramBotClientFactory botFactory
) : BackgroundService
{
    /// <summary>
    ///     <inheritdoc cref="TimeZoneDbProviderCachedInstance"/>
    /// </summary>
    private readonly TimeZoneDbProviderCachedInstance _timeCache = timeCache;

    /// <summary>
    ///     <inheritdoc cref="IDbConnectionFactory"/>
    /// </summary>
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

    /// <summary>
    ///     <inheritdoc cref="TelegramBotClientFactory"/>
    /// </summary>
    private readonly TelegramBotClientFactory _botFactory = botFactory;

    /// <summary>
    ///     <inheritdoc cref="Serilog.ILogger"/>
    /// </summary>
    private readonly Serilog.ILogger _logger = logger;

    /// <summary>
    /// Название текущего класса
    /// </summary>
    private const string CONTEXT = "Background процесс отправки задач.";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Получение списка временных зон.
                ITimeZoneOfCurrentTime[] times = Times();
                if (times.Length == 0)
                {
                    _logger.Warning("{Context}. Временных зон нет.", CONTEXT);
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                    continue;
                }

                // Получение списка получателей на основе временных зон.
                GeneralChatReceiverOfCurrentTimeZone[] receivers = await Receivers(times);

                // Получение списка тем, куда нужно отправить сообщения.
                IThemeChatTaskToFire[] themeChatTasksToFire = ThemeChatTasks(receivers);

                // Получение списка чатов, куда нужно отправить сообщения.
                IGeneralChatTaskToFire[] generalChatTasksToFire = GeneralChatTasks(receivers);

                // Отправка сообщений.
                int fired = await HandleTasksToFire(themeChatTasksToFire, generalChatTasksToFire);

                _logger.Information("{Context}. Отправлено: {Count} задач.", CONTEXT, fired);

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.Fatal("{Context} Исключение: {Message}", CONTEXT, ex.Message);
            }
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
        IThemeChatTaskToFire[] tasks = [.. receivers.SelectMany(r => r.ThemeChatTasksToFire())];
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
        IGeneralChatTaskToFire[] tasks = [.. receivers.SelectMany(s => s.GeneralChatTasksToFire())];
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

        // если не удалось отправить уведомление (когда пользователь выгнал бота из чата - удаляем чат и пользователя).
        foreach (TaskFromRemovedChat removed in unfired)
            await removed.HandleTaskFromRemovedChat(_connectionFactory);

        return count;
    }
}
