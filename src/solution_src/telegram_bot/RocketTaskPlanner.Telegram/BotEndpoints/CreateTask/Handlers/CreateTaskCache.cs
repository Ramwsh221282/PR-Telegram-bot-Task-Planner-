using PRTelegramBot.Interfaces;

namespace RocketTaskPlanner.Telegram.BotEndpoints.CreateTask.Handlers;

public sealed class CreateTaskCache : ITelegramCache
{
    public long ChatId { get; }
    public int? ThemeId { get; }

    public CreateTaskCache(long chatId, int? messageThreadId)
    {
        ChatId = chatId;
        ThemeId = messageThreadId;
    }

    public bool ClearData()
    {
        return true;
    }
}
