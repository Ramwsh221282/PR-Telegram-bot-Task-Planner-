using PRTelegramBot.Attributes;
using PRTelegramBot.Models.Enums;
using RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChat;
using RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChatTheme;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetNotificationReceiverTimeInformation;
using RocketTaskPlanner.Telegram.BotEndpoints.CreateTask.Handlers;
using RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.CreateTask;

[BotHandler]
public sealed class CreateTaskEndpoint
{
    private readonly DispatchHandler _dispatcher;

    public CreateTaskEndpoint(
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
    )
    {
        _dispatcher = new DispatchHandler(
            facade,
            timeCalculation,
            getCurrentTimeQuery,
            createTaskForChatHandler,
            createTaskForChatTheme
        );
    }

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
