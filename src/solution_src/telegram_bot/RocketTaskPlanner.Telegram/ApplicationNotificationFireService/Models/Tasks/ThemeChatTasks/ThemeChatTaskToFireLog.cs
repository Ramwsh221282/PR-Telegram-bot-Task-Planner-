namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks.ThemeChatTasks;

public sealed class ThemeChatTaskToFireLog : IThemeChatTaskToFire
{
    private readonly IThemeChatTaskToFire _task;
    private readonly Serilog.ILogger _logger;

    public ThemeChatTaskToFireLog(IThemeChatTaskToFire task, Serilog.ILogger logger)
    {
        _task = task;
        _logger = logger;
    }

    public async Task<ITaskToFire> Fire()
    {
        _logger.Information("Fired theme chat subject information:");
        SubjectId();
        ChatId();
        ThemeChatId();
        Created();
        Notified();
        IsPeriodic();
        Message();
        return await _task.Fire();
    }

    public long SubjectId()
    {
        long id = _task.SubjectId();
        _logger.Information("Theme chat subject id: {Id}", id);
        return id;
    }

    public long ChatId()
    {
        long id = _task.ChatId();
        _logger.Information("Theme chat subject chat id: {Id}", id);
        return id;
    }

    public string Message()
    {
        string message = _task.Message();
        _logger.Information("Theme chat subject message: {Message}", message);
        return message;
    }

    public int ThemeChatId()
    {
        int id = _task.ThemeChatId();
        _logger.Information("Theme chat subject id: {Id}", id);
        return id;
    }

    public DateTime Created()
    {
        DateTime created = _task.Created();
        _logger.Information("Theme chat subject created: {Created}", created);
        return created;
    }

    public DateTime Notified()
    {
        DateTime notified = _task.Notified();
        _logger.Information("Theme chat subject notified: {Notified}", notified);
        return notified;
    }

    public bool IsPeriodic()
    {
        bool isPeriodic = _task.IsPeriodic();
        _logger.Information("Theme chat subject is periodic: {IsPeriodic}", isPeriodic);
        return isPeriodic;
    }
}
