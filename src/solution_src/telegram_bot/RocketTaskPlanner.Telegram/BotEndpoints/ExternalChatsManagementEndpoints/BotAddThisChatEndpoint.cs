using PRTelegramBot.Attributes;
using PRTelegramBot.Extensions;
using PRTelegramBot.Models.Enums;
using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.Facades;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotConstants;
using RocketTaskPlanner.Telegram.BotEndpoints.ExternalChatsManagementEndpoints.Handlers.AddGeneralChat;
using RocketTaskPlanner.Telegram.BotEndpoints.ExternalChatsManagementEndpoints.Handlers.AddThemeChat;
using RocketTaskPlanner.Telegram.BotEndpoints.ExternalChatsManagementEndpoints.Handlers.DispatchHandler;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.ExternalChatsManagementEndpoints;

/// <summary>
/// Обработчик добавления чата в бота.
/// </summary>
[BotHandler]
public sealed class BotAddThisChatEndpoint
{
    private readonly Serilog.ILogger _logger;
    private const string Context = "Endpoint добавить чат.";

    /// <summary>
    /// <inheritdoc cref="TelegramBotExecutionContext"/>
    /// </summary>
    private readonly TelegramBotExecutionContext _context;

    /// <summary>
    /// <inheritdoc cref="AddGeneralChatAfterCitySelection"/>
    /// </summary>
    private readonly AddGeneralChatAfterCitySelection _handler;

    /// <summary>
    /// Конструктор endpoint
    /// <param name="timeProviders">
    ///     <inheritdoc cref="IApplicationTimeRepository{TProvider}"/>
    /// </param>
    /// <param name="firstRegistrationFacade">
    ///     <inheritdoc cref="FirstUserChatRegistrationFacade"/>
    /// </param>
    /// <param name="userChatRegistrationFacade">
    ///     <inheritdoc cref="UserChatRegistrationFacade"/>
    /// </param>
    /// <param name="userThemeRegistrationFacade">
    ///     <inheritdoc cref="UserThemeRegistrationFacade"/>
    /// </param>
    /// <param name="chats">Хранилище чатов</param>
    /// </summary>
    public BotAddThisChatEndpoint(
        IApplicationTimeRepository<TimeZoneDbProvider> timeProviders,
        IServiceScopeFactory scopeFactory,
        IExternalChatsReadableRepository chats,
        Serilog.ILogger logger
    )
    {
        _logger = logger;
        TelegramBotExecutionContext context = new();

        // обработчик для определния, какой чат добавлять (основной или темы)
        // так же в нём происходит валидация на дубликаты чатов и тем чатов по их ID.
        var dispatcher = new DispatchAddThisChatHandler(context, timeProviders, chats);

        // обработчик для создания меню выбора временной зоны.
        var generalChatHandler = new ReplyTimeZoneSelectMenu(context);

        // обработчик для добавления темы чата.
        var themeChatHandler = new AddThemeChatHandler(context, scopeFactory);

        _context = context
            .SetEntryPointHandler(dispatcher)
            .RegisterHandler(generalChatHandler)
            .RegisterHandler(themeChatHandler);

        // обработчик для добавления основного чата после выбора временной зоны
        _handler = new AddGeneralChatAfterCitySelection(scopeFactory, chats);
    }

    /// <summary>
    /// Вызов endpoint при команде /add_this_chat
    /// </summary>
    /// <param name="client">Telegram bot клиент для общения с telegram</param>
    /// <param name="update">Последнее событие (в данном случае вызов команды /add_this_chat)</param>
    [ReplyMenuHandler(CommandComparison.Contains, "/add_this_chat@", "/add_this_chat")]
    public async Task OnAddThisChat(ITelegramBotClient client, Update update)
    {
        _logger.Information("{Context}. Вызван.", Context);
        await _context.InvokeEntryPoint(client, update);
    }

    /// <summary>
    /// Обработчик нажатия отмены при выборе временной зоны
    /// </summary>
    /// <param name="botClient">Telegram bot для общения с telegram</param>
    /// <param name="update">Последнее событие (в данном случае нажатие на кнопку отменить)</param>
    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Cancellation)]
    public async Task OnCancelCitySelection(ITelegramBotClient botClient, Update update)
    {
        _logger.Information("{Context}. Операция отменена пользователем.", Context);
        update.ClearStepUserHandler();
        update.ClearCacheData();
        await PRTelegramBot.Helpers.Message.Send(
            botClient,
            update,
            ReplyMessageConstants.OperationCanceled
        );
    }

    // Далее обработчики нажатия на кнопку для каждой временной зоны.

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Anadyr)]
    public async Task OnAnadyrCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Anadyr, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Chita)]
    public async Task OnChitaCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Chita, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Kamchatka)]
    public async Task OnKamchatkaCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Kamchatka, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Krasnoyarsk)]
    public async Task OnKrasnoyarskCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Krasnoyarsk, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Novokuzneck)]
    public async Task OnNovokuzneckCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Novokuzneck, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Omsk)]
    public async Task OnOmskCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Omsk, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Srednekolysmk)]
    public async Task OnSrednekolymskCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Srednekolysmk, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.UstNera)]
    public async Task OnUstNeraCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.UstNera, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Yakutsk)]
    public async Task OnYakutskCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Yakutsk, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Astrahan)]
    public async Task OnAstrahanCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Astrahan, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Kirov)]
    public async Task OnKirovCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Kirov, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Samara)]
    public async Task OnSamaraCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Samara, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Ulyanovsk)]
    public async Task OnUlyanovskCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Ulyanovsk, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Barnaul)]
    public async Task OBarnaulCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Barnaul, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Irkutsk)]
    public async Task OnOrkutskCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Irkutsk, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Hadyga)]
    public async Task OnHadygaCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Hadyga, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Magadan)]
    public async Task OnMagadanCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Magadan, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Novosibirsk)]
    public async Task OnNovosibirskCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Novosibirsk, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Sahalin)]
    public async Task OnSahalinCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Sahalin, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Tomsk)]
    public async Task OnTomskCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Tomsk, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Vladivostok)]
    public async Task OnVladivostokCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Vladivostok, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Ekatirenburg)]
    public async Task OnEkatirenburgCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Ekatirenburg, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Kaliningrad)]
    public async Task OnKaliningradCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Kaliningrad, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Moskwa)]
    public async Task OnMoskwaCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Moskwa, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Saratov)]
    public async Task OnSaratovCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Saratov, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Volgograd)]
    public async Task OnVolgogradCitySelection(ITelegramBotClient botClient, Update update) =>
        await _handler.Handle(AddGeneralChatCitiesEnum.Saratov, botClient, update);
}
