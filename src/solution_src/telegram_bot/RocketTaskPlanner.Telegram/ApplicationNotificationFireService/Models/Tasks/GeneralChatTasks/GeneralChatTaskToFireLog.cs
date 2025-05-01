namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks.GeneralChatTasks;

public sealed class GeneralChatTaskToFireLog : IGeneralChatTaskToFire
{
    private readonly Serilog.ILogger _logger;
    private readonly IGeneralChatTaskToFire _task;

    public GeneralChatTaskToFireLog(IGeneralChatTaskToFire task, Serilog.ILogger logger)
    {
        _logger = logger;
        _task = task;
    }

    public async Task<ITaskToFire> Fire()
    {
        ITaskToFire fired = await _task.Fire();
        _logger.Information("General chat fired task information:");
        SubjectId();
        ChatId();
        Message();
        return fired;
    }

    public long SubjectId()
    {
        long id = _task.SubjectId();
        _logger.Information("General chat fired task subject id: {Id}", id);
        return id;
    }

    public long ChatId()
    {
        long id = _task.ChatId();
        _logger.Information("General chat fired task chat id: {Id}", id);
        return id;
    }

    public string Message()
    {
        string message = _task.Message();
        _logger.Information("General chat fired task message: {Message}", message);
        return message;
    }

    public DateTime Created()
    {
        DateTime created = _task.Created();
        _logger.Information("General chat fired task created: {Created}", created);
        return created;
    }

    public DateTime Notified()
    {
        DateTime notified = _task.Notified();
        _logger.Information("General chat fired task notified: {Notified}", notified);
        return notified;
    }

    public bool IsPeriodic()
    {
        bool isPeriodic = _task.IsPeriodic();
        _logger.Information("General chat fired task is periodic: {Is}", isPeriodic);
        return isPeriodic;
    }
}
