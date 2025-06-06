using System.Data;
using Dapper;
using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Database.ApplicationTimeContext.Queries.HasTimeZoneDbToken;

/// <summary>
/// Обработчик для <inheritdoc cref="HasTimeZoneDbTokenQuery"/>
/// </summary>
/// <param name="factory">Фабрика создания временных зон</param>
public sealed class HasTimeZoneDbTokenQueryHandler(IDbConnectionFactory factory)
    : IQueryHandler<HasTimeZoneDbTokenQuery, HasTimeZoneDbTokenQueryResponse>
{
    private const string Sql = """
        SELECT COUNT(*) FROM time_zone_db_providers
        """;
    private readonly IDbConnectionFactory _factory = factory;

    public async Task<HasTimeZoneDbTokenQueryResponse> Handle(
        HasTimeZoneDbTokenQuery query,
        CancellationToken ct = default
    )
    {
        CommandDefinition command = new(Sql, cancellationToken: ct);

        using IDbConnection connection = _factory.Create();
        int count = await connection.ExecuteScalarAsync<int>(command);

        return count == 0
            ? new HasTimeZoneDbTokenQueryResponse(false)
            : new HasTimeZoneDbTokenQueryResponse(true);
    }
}
