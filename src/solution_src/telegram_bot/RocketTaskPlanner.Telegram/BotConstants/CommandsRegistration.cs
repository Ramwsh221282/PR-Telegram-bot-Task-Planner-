using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotConstants;

/// <summary>
/// Метод для регистрации слэш команд в боте
/// </summary>
public static class CommandRegistration
{
    /// <summary>
    /// Регистрация отображения команд в боте
    /// </summary>
    /// <param name="client">Telegram bot клиент для общения с телеграм</param>
    public static async Task RegisterBotCommands(this ITelegramBotClient client)
    {
        await client.DeleteMyCommands();
        BotCommand[] commands =
        [
            new("/add_this_chat", "Связать чат с ботом. Используется не в чате с ботом."),
            new("/remove_this_chat", "Удаление чата из бота. Используется не в чате с ботом."),
            new(
                "/change_time_zone",
                "Изменить временную зону чата. Используется не в чате с ботом."
            ),
            new("/tc", "Создать задачу /tc <Текст задачи>. Используется не в чате с ботом."),
            new("/bot_chat_time", "Узнать временную зону чата. Используется не в чате с ботом."),
            new("/external_chat_info", "Справка по взаимодействию со своими чатами и ботом."),
            new("/bot_chat_info", "Справка по взаимодействию в приватном чате бота"),
            new("/my_tasks", "Управление созданными задачами. Используется в приват чате бота"),
        ];
        await client.SetMyCommands(commands);
    }
}
