using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Telegram.BotEndpoints.ExternalChatsManagementEndpoints.Handlers.AddGeneralChat;

namespace RocketTaskPlanner.Telegram.BotEndpoints.ChangeTimeZoneEndpoint;

/// <summary>
/// Аналогично <inheritdoc cref="AddGeneralChatScopeInfo"/>
/// </summary>
public sealed record ChangeChatTimeZoneScopeInfo(long ChatId, ApplicationTimeZone Time);
