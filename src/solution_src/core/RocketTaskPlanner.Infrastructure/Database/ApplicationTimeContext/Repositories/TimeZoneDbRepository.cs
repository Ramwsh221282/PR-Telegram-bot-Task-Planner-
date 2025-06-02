using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Database.ApplicationTimeContext.Entities;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Infrastructure.Database.ApplicationTimeContext.Repositories;

/// <summary>
/// Абстракция для работы с БД провайдера временных зон
/// </summary>
/// <param name="factory">Фабрика создания соединений с БД</param>
public sealed class TimeZoneDbRepository : IApplicationTimeRepository<TimeZoneDbProvider>
{
    private const string AddSql = """
                                  INSERT INTO time_zone_db_providers (time_zone_db_provider_id)
                                  VALUES (@time_zone_db_provider_id)
                                  """;
    private const string HasSql = "SELECT COUNT(*) FROM time_zone_db_providers";
    private const string GetSql = """
                                  SELECT
                                      time_zone_db_provider_id,
                                      time_zone_id,
                                      provider_id,
                                      time_zone_name,
                                      time_zone_date_time,
                                      time_zone_time_stamp
                                  FROM
                                      time_zone_db_providers
                                  LEFT JOIN time_zones ON provider_id = time_zone_db_provider_id
                                  """;
    private const string RemoveSql = "DELETE FROM time_zone_db_providers";
    private const string InsertZonesSql = """
                                          INSERT INTO time_zones (time_zone_id, provider_id, time_zone_name, time_zone_date_time, time_zone_time_stamp)
                                          VALUES {0}
                                          """;

    private const string CleanTimeZonesSql = """
                                             DELETE FROM time_zones
                                             """;
    
    private readonly IDbConnectionFactory _factory;

    public TimeZoneDbRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result> Add(TimeZoneDbProvider provider, CancellationToken ct = default)
    {
        var parameters = new { time_zone_db_provider_id = provider.Id.Id };
        CommandDefinition command = new(AddSql, parameters, cancellationToken: ct);
        using IDbConnection connection = _factory.Create();
        await connection.ExecuteAsync(command);
        return Result.Success();
    }

    public async Task<bool> Contains(CancellationToken ct = default)
    {
        CommandDefinition command = new(HasSql, cancellationToken: ct);
        using IDbConnection connection = _factory.Create();
        int count = await connection.ExecuteScalarAsync<int>(command);
        return count > 0;
    }

    public async Task<Result<TimeZoneDbProvider>> Get(CancellationToken ct = default)
    {
        Dictionary<string, TimeZoneDbProviderEntity> dictionary = [];
        CommandDefinition command = new(GetSql, cancellationToken: ct);
        using IDbConnection connection = _factory.Create();
        await connection.QueryAsync<
            TimeZoneDbProviderEntity,
            TimeZoneEntity,
            TimeZoneDbProviderEntity
        >(
            command,
            (provider, zone) =>
            {
                if (
                    !dictionary.TryGetValue(
                        provider.TimeZoneDbProviderId,
                        out TimeZoneDbProviderEntity? providerEntry
                    )
                )
                {
                    providerEntry = provider;
                    providerEntry.TimeZones = [];
                    dictionary.Add(providerEntry.TimeZoneDbProviderId, providerEntry);
                }

                if (zone != null)
                {
                    zone.ProviderId = providerEntry.TimeZoneDbProviderId;
                    providerEntry.TimeZones.Add(zone);
                }

                return providerEntry;
            },
            splitOn: "time_zone_id"
        );
        return dictionary.Count == 0
            ? Result.Failure<TimeZoneDbProvider>("Time Zone Db провайдер не сконфигурирован.")
            : dictionary.Values.First().ToTimeZoneDbProvider();
    }

    public async Task Remove(CancellationToken ct = default)
    {
        CommandDefinition command = new(RemoveSql, cancellationToken: ct);
        using IDbConnection connection = _factory.Create();
        await connection.ExecuteAsync(command);
    }

    

    public async Task Save(TimeZoneDbProvider provider, CancellationToken ct = default)
    { 
        using IDbConnection connection = _factory.Create();
        CommandDefinition command = new(CleanTimeZonesSql, cancellationToken: ct);
        await connection.ExecuteAsync(command);
        await InsertNewTimeZones(provider, connection, ct);
    }

    private async Task InsertNewTimeZones(
        TimeZoneDbProvider provider,
        IDbConnection connection,
        CancellationToken ct
    )
    {
        List<string> valueList = [];
        DynamicParameters parameters = new DynamicParameters();
        int index = 0;
        foreach (ApplicationTimeZone time in provider.TimeZones)
        {
            valueList.Add(
                $"(@time_zone_id{index}, @provider_id{index}, @time_zone_name{index}, @time_zone_date_time{index}, @time_zone_time_stamp{index})"
            );
            parameters.Add($"time_zone_id{index}", time.Id.Id);
            parameters.Add($"provider_id{index}", time.ProviderId.Id);
            parameters.Add($"time_zone_name{index}", time.Name.Name);
            parameters.Add($"time_zone_date_time{index}", time.TimeInfo.DateTime);
            parameters.Add($"time_zone_time_stamp{index}", time.TimeInfo.TimeStamp);
            index++;
        }

        string finalQuery = string.Format(InsertZonesSql, string.Join(", ", valueList));
        CommandDefinition command = new(finalQuery, parameters, cancellationToken: ct);
        await connection.ExecuteAsync(command);
    }
}
