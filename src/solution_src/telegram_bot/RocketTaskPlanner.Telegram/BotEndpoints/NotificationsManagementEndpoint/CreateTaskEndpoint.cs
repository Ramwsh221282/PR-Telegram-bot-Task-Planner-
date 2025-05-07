using CSharpFunctionalExtensions;
using PRTelegramBot.Attributes;
using PRTelegramBot.Extensions;
using PRTelegramBot.Models.Enums;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.NotificationsContext.Visitor;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetNotificationReceiverTimeInformation;
using RocketTaskPlanner.Telegram.BotEndpoints.NotificationsManagementEndpoint.Handlers;
using RocketTaskPlanner.Telegram.BotExtensions;
using RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.NotificationsManagementEndpoint;

/// <summary>
/// Endpoint для создания уведомлений
/// </summary>
[BotHandler]
public sealed class CreateTaskEndpoint
{
    /// <summary>
    /// <inheritdoc cref="CreateTaskHandler"/>
    /// </summary>
    private readonly CreateTaskHandler _dispatcher;

    /// <summary>
    /// <inheritdoc cref="IExternalChatsReadableRepository"/>
    /// </summary>
    private readonly IExternalChatsReadableRepository _repository;

    /// <summary>
    /// Создание endpoint для создания уведомлений
    /// <param name="facade">
    ///     <inheritdoc cref="TimeRecognitionFacade"/>
    /// </param>
    /// <param name="timeCalculation">
    ///     <inheritdoc cref="TimeCalculationService"/>
    /// </param>
    /// <param name="getCurrentTimeQuery">
    ///     <inheritdoc cref="GetNotificationReceiverTimeInformationQuery"/>
    /// </param>
    /// <param name="notificationUseCases">
    ///     <inheritdoc cref="INotificationUseCaseVisitor"/>
    /// </param>
    /// <param name="repository">
    ///     <inheritdoc cref="IExternalChatsReadableRepository"/>
    /// </param>
    /// </summary>
    public CreateTaskEndpoint(
        TimeRecognitionFacade facade,
        TimeCalculationService timeCalculation,
        IQueryHandler<
            GetNotificationReceiverTimeInformationQuery,
            GetNotificationReceiverTimeInformationQueryResponse
        > getCurrentTimeQuery,
        INotificationUseCaseVisitor notificationUseCases,
        IExternalChatsReadableRepository repository
    )
    {
        _repository = repository;
        _dispatcher = new CreateTaskHandler(
            facade,
            timeCalculation,
            notificationUseCases,
            getCurrentTimeQuery
        );
    }

    /// <summary>
    /// Вызов endpoint для создания задачи
    /// </summary>
    /// <param name="client">Telegram bot клиент для общения с telegram</param>
    /// <param name="update">Последнее событие</param>
    [SlashHandler(CommandComparison.Contains, StringComparison.OrdinalIgnoreCase, ["/tc"])]
    public async Task CreateTask(ITelegramBotClient client, Update update)
    {
        var user = update.GetUser();
        if (user.IsFailure)
            return;

        long chatId = update.GetChatId();
        Result<int> themeId = update.GetThemeId();

        // если пользователь, который вызывает эту команду, не добавлял чат в бота.
        if (!await _repository.UserOwnsChat(user.Value.Id, chatId))
        {
            await SendUserDoesntOwnChat(client, chatId, themeId);
            return;
        }

        await _dispatcher.Handle(client, update);
    }

    /// <summary>
    /// Отправка ошибки, что пользователь не является обладтелем чата.
    /// </summary>
    /// <param name="client">Telegram Bot клиент для общения с Telegram</param>
    /// <param name="chatId">ID чата</param>
    /// <param name="themeId">ID темы</param>
    private static async Task SendUserDoesntOwnChat(
        ITelegramBotClient client,
        long chatId,
        Result<int> themeId
    )
    {
        const string replyMessage = "Команда доступна тому, кто добавлял чат.";

        Task reply = themeId.IsSuccess
            ? client.SendMessage(chatId: chatId, text: replyMessage, messageThreadId: themeId.Value)
            : client.SendMessage(chatId: chatId, text: replyMessage);

        await reply;
    }
}
