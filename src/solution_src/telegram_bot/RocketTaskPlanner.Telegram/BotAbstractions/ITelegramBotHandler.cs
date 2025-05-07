using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotAbstractions;

/// <summary>
/// Интерфейс для создания классов, в которых будет обрабатываться логика ответа на команду телеграм-бота.
/// Нужен для того, чтобы регистрировать обработчики в контексте выполнения какой-либо команды.
/// Рекомендуется использовать в контекстах, если логика выполнения команды сложная.
/// </summary>
public interface ITelegramBotHandler
{
    /// <summary>
    /// Название обработчика, чтобы его можно было получать из контекста обработки.
    /// </summary>
    public string Command { get; }

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="client">Клиент телеграм-бота для общения с telegram</param>
    /// <param name="update">Событие</param>
    /// <returns>Task выполнения логики</returns>
    Task Handle(ITelegramBotClient client, Update update);
}
