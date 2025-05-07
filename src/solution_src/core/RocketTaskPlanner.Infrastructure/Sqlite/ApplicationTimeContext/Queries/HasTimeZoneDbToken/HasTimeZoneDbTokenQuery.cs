using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Queries.HasTimeZoneDbToken;

/// <summary>
/// Запрос на проверку есть ли провайдер временных зон в БД.
/// </summary>
public sealed record HasTimeZoneDbTokenQuery : IQuery;
