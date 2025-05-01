using CSharpFunctionalExtensions;
using PRTelegramBot.Extensions;
using PRTelegramBot.Models;
using RocketTaskPlanner.Application.ApplicationTimeContext.Features.SaveTimeZoneDbApiKey;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotConstants;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.StartEndpoint.Handlers.OnTokenReply;

public sealed class OnTimeZoneDbTokenReplyHandler(
    TelegramBotExecutionContext context,
    IUseCaseHandler<SaveTimeZoneDbApiKeyUseCase, TimeZoneDbProvider> useCaseHandler
) : ITelegramBotHandler
{
    private readonly TelegramBotExecutionContext _context = context;
    private readonly IUseCaseHandler<
        SaveTimeZoneDbApiKeyUseCase,
        TimeZoneDbProvider
    > _useCaseHandler = useCaseHandler;
    public string Command => TimeZoneDbApiKeyManagementConstants.TokenReplyCommand;

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        Result<string> message = update.GetMessage();
        if (message.IsFailure)
            return;

        await update.RemoveLastMessage(client);

        StepTelegram? previous = update.GetStepHandler<StepTelegram>();
        if (previous == null)
            return;

        SaveTimeZoneDbApiKeyUseCase useCase = new(message.Value);
        Result<TimeZoneDbProvider> provider = await _useCaseHandler.Handle(useCase);

        if (provider.IsFailure)
        {
            await provider.SendError(client, update);
            return;
        }

        await client.RegisterBotStartCommands();
        _context.ClearHandlers(update);
        _context.ClearCacheData(update);

        OptionMessage replyMessageOption = new() { ClearMenu = true };
        await PRTelegramBot.Helpers.Message.Send(
            client,
            update,
            TimeZoneDbApiKeyManagementConstants.ReplyMessageOnSuccess,
            replyMessageOption
        );
    }
}
