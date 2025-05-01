using CSharpFunctionalExtensions;
using PRTelegramBot.Attributes;
using PRTelegramBot.Models.Enums;
using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterChat;
using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterTheme;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
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

[BotHandler]
public sealed class BotAddThisChatEndpoint
{
    private readonly TelegramBotExecutionContext _context;
    private readonly IUseCaseHandler<
        RegisterChatUseCase,
        RegisterChatUseCaseResponse
    > _chatUseCaseHandler;

    public BotAddThisChatEndpoint(
        IApplicationTimeRepository<TimeZoneDbProvider> providerRepository,
        IUseCaseHandler<RegisterChatUseCase, RegisterChatUseCaseResponse> chatUseCaseHandler,
        IUseCaseHandler<RegisterThemeUseCase, RegisterThemeResponse> themeUseCaseHandler,
        IQueryHandler<HasNotificationReceiverQuery, bool> hasReceiverHandler,
        IQueryHandler<HasNotificationReceiverThemeQuery, bool> hasReceiverThemeHandler
    )
    {
        _chatUseCaseHandler = chatUseCaseHandler;
        TelegramBotExecutionContext context = new();
        DispatchAddThisChatHandler dispatcher = new(
            context,
            providerRepository,
            hasReceiverHandler,
            hasReceiverThemeHandler
        );
        AddGeneralChatHandler generalChatHandler = new(context, chatUseCaseHandler);
        AddThemeChatHandler themeChatHandler = new(context, themeUseCaseHandler);
        _context = context
            .SetEntryPointHandler(dispatcher)
            .RegisterHandler(generalChatHandler)
            .RegisterHandler(themeChatHandler);
    }

    [ReplyMenuHandler(
        CommandComparison.Contains,
        StringComparison.OrdinalIgnoreCase,
        commands: ["/add_this_chat"]
    )]
    public async Task OnAddThisChat(ITelegramBotClient client, Update update) =>
        await _context.InvokeEntryPoint(client, update);

    private async Task HandleCitySelection(
        AddGeneralChatCitiesEnum selector,
        ITelegramBotClient client,
        Update update
    )
    {
        AddGeneralChatScopeInfo information = AddGeneralChatScope.GetScopeInfo(selector);
        RegisterChatUseCase useCase = information.AsUseCase();
        Result<RegisterChatUseCaseResponse> response = await _chatUseCaseHandler.Handle(useCase);

        if (response.IsFailure)
        {
            await response.SendError(client, update);
            return;
        }

        await PRTelegramBot.Helpers.Message.Send(
            client,
            update,
            $"Чат {response.Value.Information()} подписан."
        );
    }

    [InlineCallbackHandler<AddGeneralChatCitiesEnum>(AddGeneralChatCitiesEnum.Cancellation)]
    public async Task OnCancelCitySelection(ITelegramBotClient botClient, Update update)
    {
        await PRTelegramBot.Helpers.Message.Send(
            botClient,
            update,
            ReplyMessageConstants.OperationCanceled
        );
    }

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
