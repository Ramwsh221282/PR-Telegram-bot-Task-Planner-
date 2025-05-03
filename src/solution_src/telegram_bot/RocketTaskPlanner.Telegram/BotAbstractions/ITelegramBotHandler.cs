using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotAbstractions;

/// <summary>
/// Интерфейс для создания классов, в которых будут обрабатываться логика ответа на команду телеграм-бота.
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
