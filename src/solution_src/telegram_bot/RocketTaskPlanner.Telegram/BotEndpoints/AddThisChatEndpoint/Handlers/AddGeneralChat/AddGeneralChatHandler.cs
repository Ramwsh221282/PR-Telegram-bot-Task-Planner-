using CSharpFunctionalExtensions;
using PRTelegramBot.Interfaces;
using PRTelegramBot.Models;
using PRTelegramBot.Models.CallbackCommands;
using PRTelegramBot.Models.InlineButtons;
using PRTelegramBot.Utils;
using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterChat;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotConstants;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RocketTaskPlanner.Telegram.BotEndpoints.AddThisChatEndpoint.Handlers.AddGeneralChat;

public sealed class AddGeneralChatHandler(
    TelegramBotExecutionContext context,
    IUseCaseHandler<RegisterChatUseCase, RegisterChatUseCaseResponse> handler
) : ITelegramBotHandler
{
    private readonly TelegramBotExecutionContext _context = context;
    private readonly IUseCaseHandler<RegisterChatUseCase, RegisterChatUseCaseResponse> _handler =
        handler;
    public string Command => AddThisChatEndpointConstants.DispatchAddThisChatHandler;

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        Result<GeneralChatCache> cache = _context.GetCacheInfo<GeneralChatCache>();
        if (cache.IsFailure)
            return;

        string chatName = cache.Value.ChatName;
        long chatId = cache.Value.ChatId;
        TimeZoneDbProvider provider = cache.Value.Provider;

        Result<IReadOnlyList<ApplicationTimeZone>> zones = await LoadTimeZones(
            provider,
            client,
            chatId
        );

        if (zones.IsFailure)
        {
            await zones.SendError(client, update);
            return;
        }

        InlineKeyboardMarkup menu = BuildTimeZoneMenu(zones.Value, chatId, chatName);
        await SendSelectTimeZoneMessage(client, update, menu);
    }

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
