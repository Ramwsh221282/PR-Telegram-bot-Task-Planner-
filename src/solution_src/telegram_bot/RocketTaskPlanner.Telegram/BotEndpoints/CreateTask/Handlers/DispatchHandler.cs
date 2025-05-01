using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChat;
using RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChatTheme;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetNotificationReceiverTimeInformation;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;
using RocketTaskPlanner.Utilities.UnixTimeUtilities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.CreateTask.Handlers;

public sealed class DispatchHandler(
    TimeRecognitionFacade facade,
    TimeCalculationService timeCalculation,
    IQueryHandler<
        GetNotificationReceiverTimeInformationQuery,
        GetNotificationReceiverTimeInformationQueryResponse
    > getCurrentTimeQuery,
    IUseCaseHandler<
        CreateTaskForChatUseCase,
        CreateTaskForChatUseCaseResponse
    > createTaskForChatHandler,
    IUseCaseHandler<
        CreateTaskForChatThemeUseCase,
        CreateTaskForChatThemeUseCaseResponse
    > createTaskForChatTheme
) : ITelegramBotHandler
{
    private readonly TimeRecognitionFacade _recognitionFacade = facade;
    private readonly TimeCalculationService _calculationService = timeCalculation;
    private readonly IQueryHandler<
        GetNotificationReceiverTimeInformationQuery,
        GetNotificationReceiverTimeInformationQueryResponse
    > _getCurrentTimeQuery = getCurrentTimeQuery;
    private readonly IUseCaseHandler<
        CreateTaskForChatUseCase,
        CreateTaskForChatUseCaseResponse
    > _createTaskForChatHandler = createTaskForChatHandler;
    private readonly IUseCaseHandler<
        CreateTaskForChatThemeUseCase,
        CreateTaskForChatThemeUseCaseResponse
    > _createTaskForChatTheme = createTaskForChatTheme;

    public string Command => CreateTaskConstants.Dispatcher;

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        Message? message = update.Message;
        if (message == null)
            return;
        string messageText = message.Text!;
        messageText = messageText.Replace("/tc", "").Trim();

        long chatId = message.Chat.Id;
        int? threadId = message.MessageThreadId;

        GetNotificationReceiverTimeInformationQuery getCurrentTime = new(chatId);
        GetNotificationReceiverTimeInformationQueryResponse currentTime =
            await _getCurrentTimeQuery.Handle(getCurrentTime);
        if (currentTime.Entity == null || currentTime.TimeZone == null)
        {
            await ReplyChatTimeIsNotConfigured(client, currentTime, chatId, threadId);
            return;
        }

        Result<TimeRecognitionResult> timeRecognitionResult =
            await _recognitionFacade.RecognizeTime(messageText);
        if (timeRecognitionResult.IsFailure)
        {
            string error = timeRecognitionResult.Error;
            await ReplyMessageTimeCannotBeRecognized(client, error, chatId, threadId);
            return;
        }

        NotificationReceiverEntity entity = currentTime.Entity;
        TimeCalculationItem offset = CalculateOffset(entity, timeRecognitionResult.Value);
        if (threadId != null)
        {
            CreateTaskForChatThemeUseCase useCase = new(
                chatId,
                threadId.Value,
                message.Id,
                currentTime.TimeZone.TimeInfo.DateTime,
                offset.CalculationDateTime,
                messageText,
                timeRecognitionResult.Value.IsPeriodic
            );
            Result<CreateTaskForChatThemeUseCaseResponse> result =
                await _createTaskForChatTheme.Handle(useCase);
            await ReplyResult(client, result, chatId, threadId.Value);
        }
        else
        {
            CreateTaskForChatUseCase useCase = new(
                chatId,
                message.Id,
                currentTime.TimeZone.TimeInfo.DateTime,
                offset.CalculationDateTime,
                messageText,
                timeRecognitionResult.Value.IsPeriodic
            );
            Result<CreateTaskForChatUseCaseResponse> result =
                await _createTaskForChatHandler.Handle(useCase);
            await ReplyResult(client, result, chatId);
        }
    }

    private static async Task ReplyChatTimeIsNotConfigured(
        ITelegramBotClient client,
        GetNotificationReceiverTimeInformationQueryResponse response,
        long chatId,
        int? messageThreadId
    )
    {
        if (messageThreadId != null)
            await client.SendMessage(
                chatId,
                text: response.Information,
                messageThreadId: messageThreadId.Value
            );
        else
            await client.SendMessage(chatId, text: response.Information);
    }

    private static async Task ReplyMessageTimeCannotBeRecognized(
        ITelegramBotClient client,
        string error,
        long chatId,
        int? messageThreadId
    )
    {
        if (messageThreadId != null)
            await client.SendMessage(chatId, text: error, messageThreadId: messageThreadId.Value);
        else
            await client.SendMessage(chatId, text: error);
    }

    private TimeCalculationItem CalculateOffset(
        NotificationReceiverEntity entity,
        TimeRecognitionResult recognition
    )
    {
        long timeStamp = entity.ReceiverZoneTimeStamp;
        DateTime dateTime = timeStamp.FromUnixTimeSeconds();
        bool isPeriodic = recognition.IsPeriodic;
        TimeCalculationItem current = new(timeStamp, dateTime, isPeriodic);
        return _calculationService.AddOffset(current, recognition);
    }

    private static async Task ReplyResult(
        ITelegramBotClient client,
        Result<CreateTaskForChatThemeUseCaseResponse> result,
        long chatId,
        int threadId
    )
    {
        if (result.IsFailure)
        {
            await client.SendMessage(chatId, text: result.Error, messageThreadId: threadId);
            return;
        }

        await client.SendMessage(chatId, text: result.Value.Information, messageThreadId: threadId);
    }

    private static async Task ReplyResult(
        ITelegramBotClient client,
        Result<CreateTaskForChatUseCaseResponse> result,
        long chatId
    )
    {
        if (result.IsFailure)
        {
            await client.SendMessage(chatId, text: result.Error);
            return;
        }

        await client.SendMessage(chatId, text: result.Value.Information);
    }
}
