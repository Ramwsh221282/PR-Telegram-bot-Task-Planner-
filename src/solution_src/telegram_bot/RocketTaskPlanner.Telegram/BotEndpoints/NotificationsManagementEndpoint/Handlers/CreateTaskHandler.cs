using CSharpFunctionalExtensions;
using PRTelegramBot.Extensions;
using RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChat;
using RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChatTheme;
using RocketTaskPlanner.Application.NotificationsContext.Visitor;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Queries.GetNotificationReceiverTimeInformation;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotExtensions;
using RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.NotificationsManagementEndpoint.Handlers;

/// <summary>
/// Обработчик создания задачи
/// </summary>
public sealed class CreateTaskHandler : ITelegramBotHandler
{
    /// <summary>
    /// <inheritdoc cref="TimeRecognitionFacade"/>
    /// </summary>
    private readonly TimeRecognitionFacade _recognitionFacade;

    /// <summary>
    /// <inheritdoc cref="TimeCalculationService"/>
    /// </summary>
    private readonly TimeCalculationService _calculationService;

    /// <summary>
    /// <inheritdoc cref="INotificationUseCaseVisitor"/>
    /// </summary>
    private readonly IServiceScopeFactory _scopeFactory;

    /// <summary>
    /// <inheritdoc cref="GetNotificationReceiverTimeInformationQueryHandler"/>
    /// </summary>
    private readonly IQueryHandler<
        GetNotificationReceiverTimeInformationQuery,
        GetNotificationReceiverTimeInformationQueryResponse
    > _getCurrentTimeQuery;

    /// <summary>
    /// <inheritdoc cref="ITelegramBotHandler.Command"/>
    /// </summary>
    public string Command => "Create task handler";

    /// <summary>
    /// Констурктор для создания обработчика создания уведомлений
    /// <param name="facade">
    ///     <inheritdoc cref="TimeRecognitionFacade"/>
    /// </param>
    /// <param name="timeCalculation">
    ///     <inheritdoc cref="TimeCalculationService"/>
    /// </param>
    /// <param name="getCurrentTimeQuery">
    ///     <inheritdoc cref="GetCurrentTime"/>
    /// </param>
    /// </summary>
    public CreateTaskHandler(
        TimeRecognitionFacade facade,
        TimeCalculationService timeCalculation,
        IServiceScopeFactory serviceScopeFactory,
        IQueryHandler<
            GetNotificationReceiverTimeInformationQuery,
            GetNotificationReceiverTimeInformationQueryResponse
        > getCurrentTimeQuery
    )
    {
        _recognitionFacade = facade;
        _calculationService = timeCalculation;
        _scopeFactory = serviceScopeFactory;
        _getCurrentTimeQuery = getCurrentTimeQuery;
    }

    /// <summary>
    /// Логика обработки создания задачи
    /// </summary>
    /// <param name="client">Telegram bot Client для взаимодействия с Telegram</param>
    /// <param name="update">Последнее событие</param>
    public async Task Handle(ITelegramBotClient client, Update update)
    {
        // форматирование сообщения, для удаления подстроки /tc
        Result<string> formattedMessage = FormattedMessage(update);
        if (formattedMessage.IsFailure)
        {
            await formattedMessage.SendError(client, update);
            return;
        }

        // получение информации о текущем времени, и времени чата.
        var timeResponse = await GetCurrentTime(update);
        if (timeResponse.IsFailure)
        {
            await timeResponse.SendError(client, update);
            return;
        }

        long chatId = update.Message!.Chat.Id;
        int? threadId = update.Message.MessageThreadId;
        long messageId = update.Message.MessageId;
        DateTime dateCreated = timeResponse.Value;

        // определение времени из текста
        var recognition = await _recognitionFacade.RecognizeTime(formattedMessage.Value);
        if (recognition.IsFailure)
        {
            string error = recognition.Error;
            await ReplyTimeCannotBeRecognized(client, error, chatId, threadId);
            return;
        }

        // рассчёт времени уведомления
        TimeCalculationItem offset = CalculateOffset(dateCreated, recognition.Value);
        bool isPeriodic = recognition.Value.IsPeriodic;
        DateTime dateNotify = offset.CalculationDateTime;

        Task chatCreation =
            threadId == null
                ? HandleForGeneralChat( // создание уведомления для основного чата
                    client,
                    chatId,
                    messageId,
                    dateCreated,
                    dateNotify,
                    formattedMessage.Value,
                    isPeriodic
                )
                : HandleForThemeChat( // создание уведомления для темы чата
                    client,
                    chatId,
                    threadId.Value,
                    messageId,
                    dateCreated,
                    dateNotify,
                    formattedMessage.Value,
                    isPeriodic
                );

        await chatCreation;
    }

    /// <summary>
    /// Метод для отправки ответа, что время не было распознано
    /// </summary>
    /// <param name="client">Telegram bot клиент для общения с telegram</param>
    /// <param name="error">Ошибка</param>
    /// <param name="chatId">Id чата</param>
    /// <param name="messageThreadId">Id темы</param>
    private static async Task ReplyTimeCannotBeRecognized(
        ITelegramBotClient client,
        string error,
        long chatId,
        int? messageThreadId
    )
    {
        if (messageThreadId != null)
            await client.SendMessage(chatId, text: error, messageThreadId: messageThreadId.Value); // отправка в тему чата
        else
            await client.SendMessage(chatId, text: error); // отправка в основной чат
    }

    /// <summary>
    /// Расчёт даты и времени уведомления.
    /// </summary>
    /// <param name="currentTime">Текущее время при создании уведомления</param>
    /// <param name="recognition">Результат распознавания времени</param>
    /// <returns>Дата и время уведомления</returns>
    private TimeCalculationItem CalculateOffset(
        DateTime currentTime,
        TimeRecognitionResult recognition
    )
    {
        bool isPeriodic = recognition.IsPeriodic;
        TimeCalculationItem current = new(currentTime, isPeriodic);
        return _calculationService.AddOffset(current, recognition);
    }

    /// <summary>
    /// Получение информации о получателе и о текущей дате и времени получателя.
    /// </summary>
    /// <param name="update">Последнее обновление для получения id чата</param>
    /// <returns>Информация о получателе и о текущей дате и времени получателе или ошибка</returns>
    private async Task<Result<DateTime>> GetCurrentTime(Update update)
    {
        long chatId = update.GetChatId();
        GetNotificationReceiverTimeInformationQuery getCurrentTime = new(chatId);
        var currentTime = await _getCurrentTimeQuery.Handle(getCurrentTime);

        if (currentTime.Entity == null || currentTime.TimeZone == null)
            return Result.Failure<DateTime>(
                "Не удалось получить время для текущего чата. Возможно временная зона не была выбрана."
            );

        DateTime dateTime = currentTime.TimeZone.TimeInfo.DateTime;
        return dateTime;
    }

    /// <summary>
    /// Метод для удаления подстроки /tc из сообщения.
    /// </summary>
    /// <param name="update">Последнее обновление для извлечения сообщения</param>
    /// <returns>Обработанное сообщение без /tc, или ошибка</returns>
    private static Result<string> FormattedMessage(Update update)
    {
        Message? message = update.Message;
        if (message == null)
            return Result.Failure<string>("Unable to get message from update");
        string messageText = message.Text!;
        return messageText.Replace("/tc", "").Trim();
    }

    /// <summary>
    /// Создание уведомления для основного чата.
    /// </summary>
    /// <param name="client">Telegram bot клиент для общения с telegram</param>
    /// <param name="chatId">id чата</param>
    /// <param name="messageId">id сообщения</param>
    /// <param name="dateCreated">дата создания</param>
    /// <param name="dateNotify">дата уведомления</param>
    /// <param name="message">текст сообщения</param>
    /// <param name="isPeriodic">периодичность сообщения</param>
    private async Task HandleForGeneralChat(
        ITelegramBotClient client,
        long chatId,
        long messageId,
        DateTime dateCreated,
        DateTime dateNotify,
        string message,
        bool isPeriodic
    )
    {
        CreateTaskForChatUseCase useCase = new(
            chatId,
            messageId,
            dateCreated,
            dateNotify,
            message,
            isPeriodic
        );
        await using var scope = _scopeFactory.CreateAsyncScope();
        var visitor = scope.ServiceProvider.GetRequiredService<INotificationUseCaseVisitor>();
        Result result = await visitor.Visit(useCase);
        if (result.IsFailure)
            await client.SendMessage(chatId, text: result.Error);
        else
            await client.SendMessage(
                chatId,
                text: ReplyMessageCreated(message, isPeriodic, dateNotify)
            );
    }

    /// <summary>
    /// Создание уведомления для темы
    /// </summary>
    /// <param name="client">Telegram bot клиент для общения с telegram</param>
    /// <param name="chatId">id чата</param>
    /// <param name="themeId">id темы</param>
    /// <param name="messageId">id сообщения</param>
    /// <param name="dateCreated">дата создания</param>
    /// <param name="dateNotify">дата уведомления</param>
    /// <param name="message">текст сообщения</param>
    /// <param name="isPeriodic">периодичность сообщения</param>
    private async Task HandleForThemeChat(
        ITelegramBotClient client,
        long chatId,
        long themeId,
        long messageId,
        DateTime dateCreated,
        DateTime dateNotify,
        string message,
        bool isPeriodic
    )
    {
        CreateTaskForChatThemeUseCase useCase = new(
            chatId,
            themeId,
            messageId,
            dateCreated,
            dateNotify,
            message,
            isPeriodic
        );
        await using var scope = _scopeFactory.CreateAsyncScope();
        var visitor = scope.ServiceProvider.GetRequiredService<INotificationUseCaseVisitor>();
        Result result = await visitor.Visit(useCase);
        if (result.IsFailure)
            await client.SendMessage(
                chatId: chatId,
                text: result.Error,
                messageThreadId: (int)themeId
            );
        else
            await client.SendMessage(
                chatId: chatId,
                text: ReplyMessageCreated(message, isPeriodic, dateNotify),
                messageThreadId: (int)themeId
            );
    }

    /// <summary>
    /// Метод создание текста сообщения о том, что уведомление было создано.
    /// </summary>
    /// <param name="messageText">Текст уведомления</param>
    /// <param name="isPeriodic">Периодичность уведомления</param>
    /// <param name="dateNotify">Дата уведомления</param>
    /// <returns>Текст сообщения о том, что уведомление было создано.</returns>
    private static string ReplyMessageCreated(
        string messageText,
        bool isPeriodic,
        DateTime dateNotify
    )
    {
        const string template = """
            Создана задача:
            {0},
            Периодичность: {1},
            Дата вызова: {2}
            """;

        string dateInfo = dateNotify.ToString("HH:mm:ss dd/MM/yyyy");
        string isPeriodicInfo = isPeriodic ? "Да" : "Нет";
        return string.Format(template, messageText, isPeriodicInfo, dateInfo);
    }
}
