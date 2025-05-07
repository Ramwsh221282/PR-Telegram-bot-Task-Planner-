namespace RocketTaskPlanner.Infrastructure.TimeZoneDb;

/// <summary>
/// Типизированный ответ временных зон от Time Zone Db провайдера
/// <param name="ZoneName">
/// Название временной зоны
/// </param>
/// <param name="TimeStamp">
/// Unix время
/// </param>
/// </summary>
public sealed record TimeZoneDbTimeResponse(string ZoneName, long TimeStamp);
