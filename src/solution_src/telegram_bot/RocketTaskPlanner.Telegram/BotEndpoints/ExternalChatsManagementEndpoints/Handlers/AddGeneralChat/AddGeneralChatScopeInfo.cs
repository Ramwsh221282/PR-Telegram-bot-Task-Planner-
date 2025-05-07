using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterChat;
using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;

namespace RocketTaskPlanner.Telegram.BotEndpoints.ExternalChatsManagementEndpoints.Handlers.AddGeneralChat;

/// <summary>
/// Dto для последующего формирования команды добавления чата.
/// </summary>
/// <param name="ChatId">Ид чата</param>
/// <param name="ChatName">Название временная зона</param>
/// <param name="Zone">Временная зона из Time Zone Db провайдера</param>
public sealed record AddGeneralChatScopeInfo(long ChatId, string ChatName, ApplicationTimeZone Zone)
{
    /// <summary>
    /// Преобразовать в Dto <inheritdoc cref="RegisterChatUseCase"/>
    /// <returns>
    ///     <inheritdoc cref="RegisterChatUseCase"/>
    /// </returns>
    /// </summary>
    public RegisterChatUseCase AsUseCase() =>
        new RegisterChatUseCase(ChatId, ChatName, Zone.Name.Name);
}
