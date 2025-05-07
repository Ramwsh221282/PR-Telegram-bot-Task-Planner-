using CSharpFunctionalExtensions;
using PRTelegramBot.Attributes;
using PRTelegramBot.Extensions;
using PRTelegramBot.Interfaces;
using PRTelegramBot.Models;
using PRTelegramBot.Models.CallbackCommands;
using PRTelegramBot.Models.Enums;
using PRTelegramBot.Models.InlineButtons;
using PRTelegramBot.Utils;
using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.NotificationsContext.Features.ChangeTimeZone;
using RocketTaskPlanner.Application.NotificationsContext.Visitor;
using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;
using RocketTaskPlanner.Telegram.BotConstants;
using RocketTaskPlanner.Telegram.BotEndpoints.ExternalChatsManagementEndpoints;
using RocketTaskPlanner.Telegram.BotEndpoints.ExternalChatsManagementEndpoints.Handlers.AddGeneralChat;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RocketTaskPlanner.Telegram.BotEndpoints.ChangeTimeZoneEndpoint;

/// <summary>
/// Endpoint телеграм бота. Который отвечает за изменение временной зоны в пользовательском внешнем основном чате.
/// </summary>
[BotHandler]
public sealed class ChangeTimeZoneEndpoint
{
    /// <summary>
    /// <inheritdoc cref="IApplicationTimeRepository{TProvider}"/>
    /// </summary>
    private readonly IApplicationTimeRepository<TimeZoneDbProvider> _providerRepository;

    /// <summary>
    /// <inheritdoc cref="IExternalChatsReadableRepository"/>
    /// </summary>
    private readonly IExternalChatsRepository _chatsRepository;

    /// <summary>
    /// <inheritdoc cref="INotificationUseCaseVisitor"/>
    /// </summary>
    private readonly INotificationUseCaseVisitor _useCases;

    public ChangeTimeZoneEndpoint(
        IApplicationTimeRepository<TimeZoneDbProvider> providerRepository,
        IExternalChatsRepository chatsRepository,
        INotificationUseCaseVisitor useCases
    )
    {
        _providerRepository = providerRepository;
        _chatsRepository = chatsRepository;
        _useCases = useCases;
    }

    /// <summary>
    /// Точка входа в endpoint.
    /// </summary>
    /// <param name="client">Telegram Bot клиент для взаимодействия с Telegram.</param>
    /// <param name="update">Последнее событие.</param>
    [SlashHandler(
        CommandComparison.Contains,
        StringComparison.OrdinalIgnoreCase,
        ["/change_time_zone@"]
    )]
    public async Task ChangeTimeZoneHandler(ITelegramBotClient client, Update update)
    {
        var user = update.GetUser();
        if (user.IsFailure)
            return;

        var chatId = update.GetChatId();
        var ownsChatTask = _chatsRepository.Readable.UserOwnsChat(user.Value.Id, chatId);
        var providerTask = _providerRepository.Get();
        await Task.WhenAll([ownsChatTask, providerTask]);

        var ownsChat = await ownsChatTask;
        var provider = await providerTask;

        // проверка на обладание чатом
        if (!ownsChat)
        {
            const string message = "Пользователь не управляет чатом, либо чат не подписан";
            await client.SendMessage(chatId: chatId, text: message);
            return;
        }

        // получение провайдера
        if (provider.IsFailure)
        {
            await client.SendMessage(chatId: chatId, text: "Не удается получить временные зоны");
            return;
        }

        // получение временных зон с обновленным временем (с настоящим временем)
        var timeZones = await LoadTimeZones(provider.Value, client, chatId);
        if (timeZones.IsFailure)
        {
            await client.SendMessage(chatId: chatId, text: "Не удается получить временные зоны");
            return;
        }

        // создание и отправка меню выбора временных зон.
        var menu = BuildTimeZoneMenu(timeZones.Value, chatId);
        await SendSelectTimeZoneMessage(client, update, menu);
    }

    /// <summary>
    /// Загрузить временные зоны
    /// </summary>
    private static async Task<Result<IReadOnlyList<ApplicationTimeZone>>> LoadTimeZones(
        TimeZoneDbProvider provider,
        ITelegramBotClient client,
        long chatId
    )
    {
        var zonesTask = provider.ProvideTimeZones();
        await client.SendMessage(chatId, text: "Загружаю меню...");
        var zones = await zonesTask;

        return zones;
    }

    /// <summary>
    /// Сделать меню выбора временных зон
    /// </summary>
    private static InlineKeyboardMarkup BuildTimeZoneMenu(
        IReadOnlyList<ApplicationTimeZone> timeZones,
        long chatId
    )
    {
        List<IInlineContent> buttons = [];

        InlineCallback<EntityTCommand<int>> cancelButton1 = new(
            ButtonTextConstants.CancelSessionButtonText,
            ChangeChatTimeZoneCitiesEnum.Cancellation
        );

        InlineCallback<EntityTCommand<int>> cancelButton2 = new(
            ButtonTextConstants.CancelSessionButtonText,
            ChangeChatTimeZoneCitiesEnum.Cancellation
        );

        buttons.Add(cancelButton1);
        buttons.Add(cancelButton2);

        // добавление кнопок выбора временных зон на основе перечисления.
        var array = Enum.GetValues<ChangeChatTimeZoneCitiesEnum>();
        for (int i = 0; i < timeZones.Count; i++)
        {
            ChangeChatTimeZoneCitiesEnum selector = array[i + 1];
            ApplicationTimeZone time = timeZones[i];

            ChangeChatTimeZoneScopeInfo information = new(chatId, time);
            ChangeGeneralChatTimeZoneScope.ManageScopeInfo(selector, information);

            string buttonText = $"{time.Name.Name} {time.TimeInfo.GetDateString()}";
            InlineCallback<EntityTCommand<int>> button = new(buttonText, selector);
            buttons.Add(button);
        }

        return MenuGenerator.InlineKeyboard(2, buttons);
    }

    /// <summary>
    /// Отправка сообщения выбора временных зон
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

    /// <summary>
    /// Обработчик нажатия отмены при выборе временной зоны
    /// </summary>
    /// <param name="botClient">Telegram bot для общения с telegram</param>
    /// <param name="update">Последнее событие (в данном случае нажатие на кнопку отменить)</param>
    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Cancellation)]
    public async Task OnCancelCitySelection(ITelegramBotClient botClient, Update update)
    {
        await PRTelegramBot.Helpers.Message.Send(
            botClient,
            update,
            ReplyMessageConstants.OperationCanceled
        );
    }

    // Обработчик, который вызывается когда выбирается временная зона.
    private async Task HandleCitySelection(
        ChangeChatTimeZoneCitiesEnum selector,
        ITelegramBotClient client,
        Update update
    )
    {
        var information = ChangeGeneralChatTimeZoneScope.GetScopeInfo(selector);
        string timeZone = information.Time.Name.Name;
        long chatId = information.ChatId;

        var useCase = new ChangeTimeZoneUseCase(chatId, timeZone);
        Result changing = await _useCases.Visit(useCase);
        if (changing.IsFailure)
        {
            await changing.SendError(client, update);
            return;
        }

        await client.SendMessage(chatId: chatId, text: $"Новая временная зона чата: {timeZone}");
    }

    // Далее обработчики нажатия на кнопку для каждой временной зоны.

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Anadyr)]
    public async Task OnAnadyrCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Anadyr, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Chita)]
    public async Task OnChitaCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Chita, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Kamchatka)]
    public async Task OnKamchatkaCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Kamchatka, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Krasnoyarsk)]
    public async Task OnKrasnoyarskCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Krasnoyarsk, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Novokuzneck)]
    public async Task OnNovokuzneckCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Novokuzneck, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Omsk)]
    public async Task OnOmskCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Omsk, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(
        ChangeChatTimeZoneCitiesEnum.Srednekolysmk
    )]
    public async Task OnSrednekolymskCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Srednekolysmk, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.UstNera)]
    public async Task OnUstNeraCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.UstNera, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Yakutsk)]
    public async Task OnYakutskCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Yakutsk, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Astrahan)]
    public async Task OnAstrahanCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Astrahan, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Kirov)]
    public async Task OnKirovCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Kirov, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Samara)]
    public async Task OnSamaraCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Samara, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Ulyanovsk)]
    public async Task OnUlyanovskCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Ulyanovsk, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Barnaul)]
    public async Task OBarnaulCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Barnaul, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Irkutsk)]
    public async Task OnOrkutskCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Irkutsk, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Hadyga)]
    public async Task OnHadygaCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Hadyga, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Magadan)]
    public async Task OnMagadanCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Magadan, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Novosibirsk)]
    public async Task OnNovosibirskCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Novosibirsk, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Sahalin)]
    public async Task OnSahalinCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Sahalin, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Tomsk)]
    public async Task OnTomskCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Tomsk, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Vladivostok)]
    public async Task OnVladivostokCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Vladivostok, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Ekatirenburg)]
    public async Task OnEkatirenburgCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Ekatirenburg, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Kaliningrad)]
    public async Task OnKaliningradCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Kaliningrad, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Moskwa)]
    public async Task OnMoskwaCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Moskwa, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Saratov)]
    public async Task OnSaratovCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Saratov, botClient, update);

    [InlineCallbackHandler<ChangeChatTimeZoneCitiesEnum>(ChangeChatTimeZoneCitiesEnum.Volgograd)]
    public async Task OnVolgogradCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(ChangeChatTimeZoneCitiesEnum.Saratov, botClient, update);
}
