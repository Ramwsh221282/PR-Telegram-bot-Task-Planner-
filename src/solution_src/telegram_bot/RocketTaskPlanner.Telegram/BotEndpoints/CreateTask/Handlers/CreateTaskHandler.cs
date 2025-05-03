using CSharpFunctionalExtensions;
using PRTelegramBot.Extensions;
using RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChat;
using RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChatTheme;
using RocketTaskPlanner.Application.NotificationsContext.Visitor;
using RocketTaskPlanner.Application.UsersContext.Features.EnsureUserHasPermissions;
using RocketTaskPlanner.Application.UsersContext.Visitor;
using RocketTaskPlanner.Domain.PermissionsContext;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetNotificationReceiverTimeInformation;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotExtensions;
using RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;
using RocketTaskPlanner.Utilities.UnixTimeUtilities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.CreateTask.Handlers;

/// <summary>
/// Обработчик создания задачи
/// </summary>
public sealed class CreateTaskHandler : ITelegramBotHandler
{
    private readonly TimeRecognitionFacade _recognitionFacade;
    private readonly TimeCalculationService _calculationService;
    private readonly INotificationUseCaseVisitor _notificationUseCases;
    private readonly IUsersUseCaseVisitor _usersUseCases;
    private readonly IQueryHandler<
        GetNotificationReceiverTimeInformationQuery,
        GetNotificationReceiverTimeInformationQueryResponse
    > _getCurrentTimeQuery;

    public string Command => "Create task handler";

    /// <summary>
    /// Констурктор для создания обработчика создания уведомлений
    /// </summary>
    /// <param name="facade">Фасадный класс для определения времени.</param>
    /// <param name="timeCalculation">Класс для расчёта времени уведомления.</param>
    /// <param name="notificationUseCases">Посетитель для логики контекста уведомлений.</param>
    /// <param name="usersUseCases">Посетитель для логики контекста пользователей.</param>
    /// <param name="getCurrentTimeQuery">Обработчик для получение текущего времени создания задачи.</param>
    public CreateTaskHandler(
        TimeRecognitionFacade facade,
        TimeCalculationService timeCalculation,
        INotificationUseCaseVisitor notificationUseCases,
        IUsersUseCaseVisitor usersUseCases,
        IQueryHandler<
            GetNotificationReceiverTimeInformationQuery,
            GetNotificationReceiverTimeInformationQueryResponse
        > getCurrentTimeQuery
    )
    {
        _recognitionFacade = facade;
        _calculationService = timeCalculation;
        _usersUseCases = usersUseCases;
        _usersUseCases = usersUseCases;
        _notificationUseCases = notificationUseCases;
        _getCurrentTimeQuery = getCurrentTimeQuery;
    }

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        // форматирование сообщения, для удаления подстроки /tc
        Result<string> formattedMessage = FormattedMessage(update);
        if (formattedMessage.IsFailure)
        {
            await formattedMessage.SendError(client, update);
            return;
        }

        // проверка на наличие права create tasks
        Result hasPermissions = await EnsureUserCanCreateTasks(update);
        if (hasPermissions.IsFailure)
        {
            await hasPermissions.SendError(client, update);
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
        NotificationReceiverEntity receiver = timeResponse.Value.receiver;
        DateTime dateCreated = timeResponse.Value.dateTime;

        // определение времени из текста
        var recognition = await _recognitionFacade.RecognizeTime(formattedMessage.Value);
        if (recognition.IsFailure)
        {
            string error = recognition.Error;
            await ReplyTimeCannotBeRecognized(client, error, chatId, threadId);
            return;
        }

        // рассчёт времени уведомления
        TimeCalculationItem offset = CalculateOffset(receiver, recognition.Value);
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
    /// <param name="entity">Получатель уведомления (основной чат)</param>
    /// <param name="recognition">Результат распознавания времени</param>
    /// <returns>Дата и время уведомления</returns>
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

    /// <summary>
    /// Проверка, что юзер может создавать задачи.
    /// </summary>
    /// <param name="update">Последнее обновление для извлечения информации о юзере - user id</param>
    /// <returns>Result Success если может создавать, Result Failure если не может создавать</returns>
    private async Task<Result> EnsureUserCanCreateTasks(Update update)
    {
        Result<TelegramBotUser> userResult = update.GetUser();
        if (userResult.IsFailure)
            return Result.Failure(userResult.Error);

        TelegramBotUser user = userResult.Value;
        long userId = user.Id;
        string[] permissions = [PermissionNames.CreateTasks];
        EnsureUserHasPermissionsUseCase useCase = new(userId, permissions);
        Result hasPermissions = await _usersUseCases.Visit(useCase);
        return hasPermissions;
    }

    /// <summary>
    /// Получение информации о получателе и о текущей дате и времени получателя.
    /// </summary>
    /// <param name="update">Последнее обновление для получения id чата</param>
    /// <returns>Информация о получателе и о текущей дате и времени получателе или ошибка</returns>
    private async Task<
        Result<(NotificationReceiverEntity receiver, DateTime dateTime)>
    > GetCurrentTime(Update update)
    {
        long chatId = update.GetChatId();
        GetNotificationReceiverTimeInformationQuery getCurrentTime = new(chatId);
        var currentTime = await _getCurrentTimeQuery.Handle(getCurrentTime);
        if (currentTime.Entity == null || currentTime.TimeZone == null)
            return Result.Failure<(NotificationReceiverEntity receiver, DateTime dateTime)>(
                "Не удалось получить время для текущего чата. Возможно оно не было сконфигурировано"
            );
        NotificationReceiverEntity entity = currentTime.Entity;
        DateTime dateTime = currentTime.TimeZone.TimeInfo.DateTime;
        return (entity, dateTime);
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
        Result result = await _notificationUseCases.Visit(useCase);
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
        Result result = await _notificationUseCases.Visit(useCase);
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
        string template = """
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
