using Telegram.Bot;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks.ThemeChatTasks;

public sealed class ThemeChatTaskToFireTelegramSpeaking : IThemeChatTaskToFire
{
    private readonly IThemeChatTaskToFire _task;
    private readonly TelegramBotClient _client;

    public ThemeChatTaskToFireTelegramSpeaking(IThemeChatTaskToFire task, TelegramBotClient client)
    {
        _task = task;
        _client = client;
    }

    public async Task<ITaskToFire> Fire()
    {
        await SendTelegramMessage();
        return await _task.Fire();
    }

    public long SubjectId() => _task.SubjectId();

    public long ChatId() => _task.ChatId();

    public string Message() => _task.Message();

    public int ThemeChatId() => _task.ThemeChatId();

    public DateTime Created() => _task.Created();

    public DateTime Notified() => _task.Notified();

    public bool IsPeriodic() => _task.IsPeriodic();

    private async Task SendTelegramMessage()
    {
        long chatId = ChatId();
        int themeId = ThemeChatId();
        string message = Message();
        await _client.SendMessage(chatId: chatId, text: message, messageThreadId: themeId);
    }
}
