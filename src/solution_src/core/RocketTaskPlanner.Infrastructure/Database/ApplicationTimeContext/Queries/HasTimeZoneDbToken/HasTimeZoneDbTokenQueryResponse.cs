namespace RocketTaskPlanner.Infrastructure.Database.ApplicationTimeContext.Queries.HasTimeZoneDbToken;

/// <summary>
/// Результат запроса <inheritdoc cref="HasTimeZoneDbTokenQuery"/>
/// </summary>
/// <param name="Has">True если провайдер есть, иначе False.</param>
public sealed record HasTimeZoneDbTokenQueryResponse(bool Has);
