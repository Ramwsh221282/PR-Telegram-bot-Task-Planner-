using PRTelegramBot.Attributes;
using PRTelegramBot.Models.Enums;
using RocketTaskPlanner.Application.NotificationsContext.Visitor;
using RocketTaskPlanner.Application.UsersContext.Visitor;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetNotificationReceiverTimeInformation;
using RocketTaskPlanner.Telegram.BotEndpoints.CreateTask.Handlers;
using RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.CreateTask;

/// <summary>
/// Endpoint для создания уведомлений
/// </summary>
[BotHandler]
public sealed class CreateTaskEndpoint
{
    /// <summary>
    /// Обработчик создания уведомления.
    /// </summary>
    private readonly CreateTaskHandler _dispatcher;

    /// <summary>
    /// Создание endpoint для создания уведомлений
    /// </summary>
    /// <param name="facade">Фасадный класс для определения времени.</param>
    /// <param name="timeCalculation">Класс для расчёта времени уведомления.</param>
    /// <param name="getCurrentTimeQuery">Обработчик для получение текущего времени создания задачи.</param>
    /// <param name="notificationUseCases">Посетитель для логики контекста уведомлений.</param>
    /// <param name="userUseCases">Посетитель для логики контекста пользователей.</param>
    public CreateTaskEndpoint(
        TimeRecognitionFacade facade,
        TimeCalculationService timeCalculation,
        IQueryHandler<
            GetNotificationReceiverTimeInformationQuery,
            GetNotificationReceiverTimeInformationQueryResponse
        > getCurrentTimeQuery,
        INotificationUseCaseVisitor notificationUseCases,
        IUsersUseCaseVisitor userUseCases
    )
    {
        _dispatcher = new CreateTaskHandler(
            facade,
            timeCalculation,
            notificationUseCases,
            userUseCases,
            getCurrentTimeQuery
        );
    }

    /// <summary>
    /// Вызов endpoint для создания задачи
    /// </summary>
    /// <param name="client">Telegram bot клиент для общения с telegram</param>
    /// <param name="update">Последнее событие</param>
    [ReplyMenuHandler(CommandComparison.Contains, StringComparison.OrdinalIgnoreCase, ["/tc"])]
    public async Task GetChatTime(ITelegramBotClient client, Update update)
    {
        try
        {
            await _dispatcher.Handle(client, update);
        }
        catch (Exception ex)
        {
            await PRTelegramBot.Helpers.Message.Send(client, update, ex.Message);
        }
    }
}
