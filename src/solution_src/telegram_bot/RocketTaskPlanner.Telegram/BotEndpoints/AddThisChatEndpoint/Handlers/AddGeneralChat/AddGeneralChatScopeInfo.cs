using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterChat;
using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;

namespace RocketTaskPlanner.Telegram.BotEndpoints.AddThisChatEndpoint.Handlers.AddGeneralChat;

public sealed record AddGeneralChatScopeInfo(long ChatId, string ChatName, ApplicationTimeZone Zone)
{
    public RegisterChatUseCase AsUseCase() =>
        new RegisterChatUseCase(ChatId, ChatName, Zone.Name.Name, Zone.TimeInfo.TimeStamp);
}
