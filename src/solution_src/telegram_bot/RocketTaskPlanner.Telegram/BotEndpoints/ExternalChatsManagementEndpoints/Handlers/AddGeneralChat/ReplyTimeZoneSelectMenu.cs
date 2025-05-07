using CSharpFunctionalExtensions;
using PRTelegramBot.Interfaces;
using PRTelegramBot.Models;
using PRTelegramBot.Models.CallbackCommands;
using PRTelegramBot.Models.InlineButtons;
using PRTelegramBot.Utils;
using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotConstants;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RocketTaskPlanner.Telegram.BotEndpoints.ExternalChatsManagementEndpoints.Handlers.AddGeneralChat;

/// <summary>
/// Обработчик для отправки меню с выбором временной зоны
/// </summary>
/// <param name="context">Контекст выполнения команды /add_this_chat</param>
public sealed class ReplyTimeZoneSelectMenu(TelegramBotExecutionContext context)
    : ITelegramBotHandler
{
    /// <summary>
    /// <inheritdoc cref="TelegramBotExecutionContext"/>
    /// </summary>
    private readonly TelegramBotExecutionContext _context = context;

    /// <summary>
    /// <inheritdoc cref="ITelegramBotHandler.Command"/>
    /// </summary>
    public string Command => AddThisChatEndpointConstants.DispatchAddThisChatHandler;

    /// <summary>
    /// Логика отправки меню выбора временных зон
    /// </summary>
    public async Task Handle(ITelegramBotClient client, Update update)
    {
        Result<GeneralChatCache> cache = _context.GetCacheInfo<GeneralChatCache>();
        if (cache.IsFailure)
            return;

        string chatName = cache.Value.ChatName;
        long chatId = cache.Value.ChatId;
        TimeZoneDbProvider provider = cache.Value.Provider;

        // получение временных зон
        var zones = await LoadTimeZones(provider, client, chatId);

        if (zones.IsFailure)
        {
            await zones.SendError(client, update);
            return;
        }

        // создание меню выбора временной зоны
        InlineKeyboardMarkup menu = BuildTimeZoneMenu(zones.Value, chatId, chatName);

        // отправка меню выбора временных зон
        await SendSelectTimeZoneMessage(client, update, menu);
    }

    /// <summary>
    /// Получить временные зоны из провайдера
    /// </summary>
    private static async Task<Result<IReadOnlyList<ApplicationTimeZone>>> LoadTimeZones(
        TimeZoneDbProvider provider,
        ITelegramBotClient client,
        long chatId
    )
    {
        Task<Result<IReadOnlyList<ApplicationTimeZone>>> zonesTask = provider.ProvideTimeZones();

        await client.SendMessage(
            chatId,
            text: AddThisChatEndpointConstants.AddThisChatSelectTimeZoneReply
        );

        await client.SendMessage(chatId, text: "Загружаю меню...");

        Result<IReadOnlyList<ApplicationTimeZone>> zones = await zonesTask;
        return zones;
    }

    /// <summary>
    /// Сделать меню выбора временных зон
    /// </summary>
    private static InlineKeyboardMarkup BuildTimeZoneMenu(
        IReadOnlyList<ApplicationTimeZone> timeZones,
        long chatId,
        string chatName
    )
    {
        List<IInlineContent> buttons = [];

        InlineCallback<EntityTCommand<int>> cancelButton1 = new(
            ButtonTextConstants.CancelSessionButtonText,
            AddGeneralChatCitiesEnum.Cancellation
        );

        InlineCallback<EntityTCommand<int>> cancelButton2 = new(
            ButtonTextConstants.CancelSessionButtonText,
            AddGeneralChatCitiesEnum.Cancellation
        );

        buttons.Add(cancelButton1);
        buttons.Add(cancelButton2);

        // добавление кнопок выбора временных зон на основе перечисления.
        AddGeneralChatCitiesEnum[] array = Enum.GetValues<AddGeneralChatCitiesEnum>();
        for (int i = 0; i < timeZones.Count; i++)
        {
            AddGeneralChatCitiesEnum selector = array[i + 1];
            ApplicationTimeZone time = timeZones[i];
            AddGeneralChatScopeInfo information = new(chatId, chatName, time);
            AddGeneralChatScope.ManageScopeInfo(selector, information);

            string buttonText = $"{time.Name.Name} {time.TimeInfo.GetDateString()}";
            InlineCallback<EntityTCommand<int>> button = new(buttonText, selector);
            buttons.Add(button);
        }

        return MenuGenerator.InlineKeyboard(2, buttons);
    }

    /// <summary>
    /// Отправить сообщения, что нужно выбрать временную зону
    /// </summary>
    private static async Task SendSelectTimeZoneMessage(
        ITelegramBotClient client,
        Update update,
        InlineKeyboardMarkup menu
    )
    {
        OptionMessage option = new() { MenuInlineKeyboardMarkup = menu };
        await PRTelegramBot.Helpers.Message.Send(
            client,
            update,
            AddThisChatEndpointConstants.SelectTimeZoneReply,
            option
        );
    }
}
