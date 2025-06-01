using CSharpFunctionalExtensions;
using PRTelegramBot.Attributes;
using PRTelegramBot.Extensions;
using PRTelegramBot.Models.Enums;
using RocketTaskPlanner.Telegram.BotConstants;
using RocketTaskPlanner.Telegram.BotExtensions;
using SQLitePCL;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.StartEndpoint;

/// <summary>
/// Endpoint обработки для команды /start
/// </summary>
[BotHandler]
public sealed class BotStartEndpoint
{
    private const string _messageOnStart = """
        Вас приветствует Telegram Bot Task Planner.

        С ботом можно работать как из внешнего чата, так и в его приватном чате.

        Для того, чтобы узнать информацию о работе с внешними чатами

        используйте команду /external_chat_info

        Для того, чтобы узнать информацию о работе в приватном чате

        используйте команду /bot_chat_info
        """;

    private const string _externalChatInfo = """
        Команды, которые можно использовать во внешних чатах бота:

        /add_this_chat - связывает чат (или тему) откуда эта команда была вызвана.
        При связывании чата, пользователь, вызвавший эту команду, становится обладателем этого чата в рамках Telegram Bot Planner. 
        Для того, чтобы можно было создавать и отправлять уведомления в нужный чат, необходимо вызвать эту команду.

        /remove_this_chat - удаляет связанный чат (или тему) окуда эта команда была вызвана.
        При удалении чата (или темы чата), уведомления, которые были запланированы для этого чата (или темы чата) удаляются.

        /change_time_zone - позволяет изменить временную зону основного чата.

        /tc <текст уведомления> - команда создаёт задачу для чата (или темы), откуда эта команда была вызвана.
        Для создания уведомления, необходимо указать дату и время. Время в формате ЧЧ:ММ или ЧЧ ММ
        Например: Сегодня сообщить боту в 15:35, что необходимо создать уведомление.

        /chat_time - узнать время текущего чата.
        """;

    private const string _botChatInfo = """
        Команды, которые можно использовать с приват чатом бота:

        /my_tasks - команда, позволяющая просмотреть список задач для каждого чата.

        Используя эту команду, можно выбрать уведомление и удалить его при необходимости.
        """;

    /// <summary>
    /// Точка входа в команду /start
    /// </summary>
    /// <param name="client">Telegram bot клиент для общения с Telegram.</param>
    /// <param name="update">Последнее событие.</param>
    [ReplyMenuHandler(CommandComparison.Equals, StringComparison.OrdinalIgnoreCase, ["/start@", "/start"])]
    public async Task OnStart(ITelegramBotClient client, Update update)
    {
        long chatId = update.GetChatId();
        Result<int> themeId = update.GetThemeId();
        await SendInformation(client, chatId, themeId, _messageOnStart);
        await client.RegisterBotCommands();
    }

    [ReplyMenuHandler(CommandComparison.Equals, StringComparison.OrdinalIgnoreCase, ["/external_chat_info@", "/external_chat_info"]
    )]
    public async Task OnExternalChatInfo(ITelegramBotClient client, Update update)
    {
        long chatId = update.GetChatId();
        Result<int> themeId = update.GetThemeId();
        await SendInformation(client, chatId, themeId, _externalChatInfo);
    }

    [SlashHandler(CommandComparison.Equals, StringComparison.OrdinalIgnoreCase, ["/bot_chat_info@", "/bot_chat_info"]
    )]
    public async Task OnBotChatInfo(ITelegramBotClient client, Update update)
    {
        long chatId = update.GetChatId();
        Result<int> themeId = update.GetThemeId();
        await SendInformation(client, chatId, themeId, _botChatInfo);
    }

    private async Task SendInformation(
        ITelegramBotClient client,
        long chatId,
        Result<int> themeId,
        string textMessage
    )
    {
        Task operation = themeId.IsSuccess switch
        {
            true => client.SendMessage(
                chatId: chatId,
                text: textMessage,
                messageThreadId: themeId.Value
            ),
            false => client.SendMessage(chatId: chatId, text: textMessage),
        };

        await operation;
    }
}
