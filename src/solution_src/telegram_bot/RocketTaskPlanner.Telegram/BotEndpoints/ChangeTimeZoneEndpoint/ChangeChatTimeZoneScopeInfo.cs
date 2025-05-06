using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;

namespace RocketTaskPlanner.Telegram.BotEndpoints.ChangeTimeZoneEndpoint;

public sealed record ChangeChatTimeZoneScopeInfo(long ChatId, ApplicationTimeZone Time);
