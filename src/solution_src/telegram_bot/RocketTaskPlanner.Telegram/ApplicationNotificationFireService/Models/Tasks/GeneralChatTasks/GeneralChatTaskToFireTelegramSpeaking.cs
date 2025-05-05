using Telegram.Bot;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Tasks.GeneralChatTasks;

/// <summary>
/// Декоратор для отправки уведомления в телеграм.
/// </summary>
public sealed class GeneralChatTaskToFireTelegramSpeaking : IGeneralChatTaskToFire
{
    private readonly IGeneralChatTaskToFire _taskToFire;
    private readonly ITelegramBotClient _botClient;

    public GeneralChatTaskToFireTelegramSpeaking(
        IGeneralChatTaskToFire taskToFire,
        ITelegramBotClient botClient
    )
    {
        _taskToFire = taskToFire;
        _botClient = botClient;
    }

    public async Task<ITaskToFire> Fire()
    {
        try
        {
            await SendTelegramBotMessage();
            return await _taskToFire.Fire();
        }
        catch
        {
            return new TaskFromRemovedChat(ChatId());
        }
    }

    public long SubjectId() => _taskToFire.SubjectId();

    public long ChatId() => _taskToFire.ChatId();

    public string Message() => _taskToFire.Message();

    public DateTime Created() => _taskToFire.Created();

    public DateTime Notified() => _taskToFire.Notified();

    public bool IsPeriodic() => _taskToFire.IsPeriodic();

    private async Task SendTelegramBotMessage()
    {
        long chatId = ChatId();
        string text = Message();
        await _botClient.SendMessage(chatId, text);
    }
}
