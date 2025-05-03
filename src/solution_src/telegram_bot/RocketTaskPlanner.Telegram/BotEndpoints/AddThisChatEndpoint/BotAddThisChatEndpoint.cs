using CSharpFunctionalExtensions;
using PRTelegramBot.Attributes;
using PRTelegramBot.Models.Enums;
using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterChat;
using RocketTaskPlanner.Application.NotificationsContext.Visitor;
using RocketTaskPlanner.Application.UsersContext.Features.AddUserWithPermissions;
using RocketTaskPlanner.Application.UsersContext.Visitor;
using RocketTaskPlanner.Domain.PermissionsContext;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.HasNotificationReceiver;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.HasNotificationReceiverTheme;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotConstants;
using RocketTaskPlanner.Telegram.BotEndpoints.AddThisChatEndpoint.Handlers.AddGeneralChat;
using RocketTaskPlanner.Telegram.BotEndpoints.AddThisChatEndpoint.Handlers.AddThemeChat;
using RocketTaskPlanner.Telegram.BotEndpoints.AddThisChatEndpoint.Handlers.DispatchHandler;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.AddThisChatEndpoint;

/// <summary>
/// Обработчик добавления чата в бота.
/// </summary>
[BotHandler]
public sealed class BotAddThisChatEndpoint
{
    // контекст для выполнения команды
    private readonly TelegramBotExecutionContext _context;

    // посетитель для выполнения команд в контексте уведомлений
    private readonly INotificationUseCaseVisitor _notificationUseCases;

    // посетитель для выполнения команд в контексте пользователей.
    private readonly IUsersUseCaseVisitor _userUseCases;

    /// <summary>
    /// Конструктор endpoint
    /// </summary>
    /// <param name="providerRepository">Репозиторий провайдера времени Time Zone Db</param>
    /// <param name="hasReceiverHandler">Обработчик для проверки на существование чата</param>
    /// <param name="hasReceiverThemeHandler">Обработчик для проверки на существование темы</param>
    /// <param name="userUseCases">Посетитель для выполнения команд в контексте пользователей</param>
    /// <param name="notificationUseCases">Посетитель для выполнения команд в контексте уведомлений</param>
    public BotAddThisChatEndpoint(
        IApplicationTimeRepository<TimeZoneDbProvider> providerRepository,
        IQueryHandler<HasNotificationReceiverQuery, bool> hasReceiverHandler,
        IQueryHandler<HasNotificationReceiverThemeQuery, bool> hasReceiverThemeHandler,
        IUsersUseCaseVisitor userUseCases,
        INotificationUseCaseVisitor notificationUseCases
    )
    {
        _userUseCases = userUseCases;
        _notificationUseCases = notificationUseCases;
        TelegramBotExecutionContext context = new();

        // обработчик для определния, какой чат добавлять (основной или темы)
        DispatchAddThisChatHandler dispatcher = new(
            context,
            providerRepository,
            hasReceiverHandler,
            hasReceiverThemeHandler
        );

        // обработчик для создания меню выбора временной зоны.
        ReplyTimeZoneSelectMenu generalChatHandler = new(context);

        // обработчик для добавления темы чата.
        AddThemeChatHandler themeChatHandler = new(context, notificationUseCases);

        _context = context
            .SetEntryPointHandler(dispatcher)
            .RegisterHandler(generalChatHandler)
            .RegisterHandler(themeChatHandler);
    }

    /// <summary>
    /// Вызов endpoint при команде /add_this_chat
    /// </summary>
    /// <param name="client">Telegram bot клиент для общения с telegram</param>
    /// <param name="update">Последнее событие (в данном случае вызов команды /add_this_chat)</param>
    [ReplyMenuHandler(
        CommandComparison.Contains,
        StringComparison.OrdinalIgnoreCase,
        commands: ["/add_this_chat"]
    )]
    public async Task OnAddThisChat(ITelegramBotClient client, Update update) =>
        await _context.InvokeEntryPoint(client, update);

    /// <summary>
    /// Обработка выбора временной зоны.
    /// </summary>
    /// <param name="selector">Выбранная временная зона.</param>
    /// <param name="client">Telegram bot для общения с telegram</param>
    /// <param name="update">Последнее событие (в данном случае нажатие на кнопку с временной зоной)</param>
    private async Task HandleCitySelection(
        AddGeneralChatCitiesEnum selector,
        ITelegramBotClient client,
        Update update
    )
    {
        Result<TelegramBotUser> userInfoResult = update.GetUserFromCallback();
        if (userInfoResult.IsFailure)
        {
            await userInfoResult.SendError(client, update);
            return;
        }

        TelegramBotUser user = userInfoResult.Value;
        AddGeneralChatScopeInfo information = AddGeneralChatScope.GetScopeInfo(selector);
        await TryAddUser(user); // попытка добавить пользователя, если он уже добавлен - игнорировать ошибку

        Result response = await RegisterChat(information); // добавление чата
        if (response.IsFailure)
        {
            await response.SendError(client, update);
            return;
        }

        await PRTelegramBot.Helpers.Message.Send(client, update, $"Чат подписан.");
        await client.RegisterBotCommands();
    }

    /// <summary>
    /// Метод добавление чата
    /// </summary>
    /// <param name="info">Информация о чате</param>
    /// <returns>Результат добавления чата. Success или Failure</returns>
    private async Task<Result> RegisterChat(AddGeneralChatScopeInfo info)
    {
        RegisterChatUseCase useCase = info.AsUseCase();
        return await _notificationUseCases.Visit(useCase);
    }

    /// <summary>
    /// Метод добавления пользователя с правами Create tasks
    /// </summary>
    /// <param name="user">Информация о пользователе</param>
    private async Task TryAddUser(TelegramBotUser user)
    {
        long userId = user.Id;
        string userName = user.CombineNamesAsNickname();
        string[] permissions = [PermissionNames.CreateTasks];
        AddUserWithPermissionsUseCase useCase = new(userId, userName, permissions);
        await _userUseCases.Visit(useCase);
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

    // Далее обработчики нажатия на кнопку для каждой временной зоны.

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Anadyr)]
    public async Task OnAnadyrCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Anadyr, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Chita)]
    public async Task OnChitaCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Chita, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Kamchatka)]
    public async Task OnKamchatkaCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Kamchatka, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Krasnoyarsk)]
    public async Task OnKrasnoyarskCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Krasnoyarsk, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Novokuzneck)]
    public async Task OnNovokuzneckCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Novokuzneck, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Omsk)]
    public async Task OnOmskCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Omsk, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Srednekolysmk)]
    public async Task OnSrednekolymskCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Srednekolysmk, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.UstNera)]
    public async Task OnUstNeraCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.UstNera, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Yakutsk)]
    public async Task OnYakutskCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Yakutsk, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Astrahan)]
    public async Task OnAstrahanCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Astrahan, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Kirov)]
    public async Task OnKirovCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Kirov, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Samara)]
    public async Task OnSamaraCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Samara, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Ulyanovsk)]
    public async Task OnUlyanovskCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Ulyanovsk, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Barnaul)]
    public async Task OBarnaulCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Barnaul, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Irkutsk)]
    public async Task OnOrkutskCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Irkutsk, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Hadyga)]
    public async Task OnHadygaCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Hadyga, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Magadan)]
    public async Task OnMagadanCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Magadan, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Novosibirsk)]
    public async Task OnNovosibirskCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Novosibirsk, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Sahalin)]
    public async Task OnSahalinCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Sahalin, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Tomsk)]
    public async Task OnTomskCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Tomsk, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Vladivostok)]
    public async Task OnVladivostokCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Vladivostok, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Ekatirenburg)]
    public async Task OnEkatirenburgCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Ekatirenburg, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Kaliningrad)]
    public async Task OnKaliningradCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Kaliningrad, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Moskwa)]
    public async Task OnMoskwaCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Moskwa, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Saratov)]
    public async Task OnSaratovCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Saratov, botClient, update);

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Volgograd)]
    public async Task OnVolgogradCitySelection(ITelegramBotClient botClient, Update update) =>
        await HandleCitySelection(AddGeneralChatCitiesEnum.Saratov, botClient, update);
}
