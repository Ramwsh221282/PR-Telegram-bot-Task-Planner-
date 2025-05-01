using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotConstants;

public static class CommandRegistration
{
    public static async Task RegisterBotStartCommands(this ITelegramBotClient client)
    {
        await client.RegisterCommandIfNotExists("/add_this_chat", "Подписывание текущего чата.");
    }

    public static async Task RegisterBotCommandsOnChatSubscribe(
        this ITelegramBotClient client,
        ChatId chatId
    )
    {
        BotCommandScopeChatAdministrators scope = BotCommandScope.ChatAdministrators(chatId);

        await client.RegisterCommandIfNotExists(
            "/time_config",
            "Управление временем текущего чата.",
            scope
        );

        await client.RegisterCommandIfNotExists("/bot_time", "Время текущего чата.");

        await client.RegisterCommandIfNotExists(
            "/task_create",
            "Создание уведомления для текущего чата."
        );
    }

    private static async Task RegisterCommandIfNotExists(
        this ITelegramBotClient client,
        string command,
        string description,
        BotCommandScope? scope = null
    )
    {
        List<BotCommand> commands = [.. await client.GetMyCommands()];
        if (!commands.Any(c => c.Command == command))
        {
            commands.Add(new BotCommand() { Command = command, Description = description });

            if (scope != null)
            {
                await client.SetMyCommands(commands, scope);
                return;
            }

            await client.SetMyCommands(commands);
        }
    }
}
