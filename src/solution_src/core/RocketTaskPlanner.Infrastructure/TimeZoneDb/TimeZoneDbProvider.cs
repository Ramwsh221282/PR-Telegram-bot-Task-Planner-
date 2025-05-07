using CSharpFunctionalExtensions;
using RocketTaskPlanner.Domain.ApplicationTimeContext;
using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Domain.ApplicationTimeContext.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.TimeZoneDb;

/// <summary>
/// Провайдер временных зон сервиса Time Zone Db
/// </summary>
public sealed class TimeZoneDbProvider : IApplicationTimeProvider
{
    /// <summary>
    /// Список <inheritdoc cref="ApplicationTimeZone"/>
    /// </summary>
    private readonly List<ApplicationTimeZone> _zones = [];

    /// <summary>
    /// Токен <inheritdoc cref="TimeZoneDbToken"/>
    /// </summary>
    public IApplicationTimeProviderId Id { get; } = null!;

    /// <summary>
    /// <inheritdoc cref="_zones"/>
    /// </summary>
    public IReadOnlyCollection<ApplicationTimeZone> TimeZones => _zones;

    private TimeZoneDbProvider() { } // ef core;

    public TimeZoneDbProvider(TimeZoneDbToken token) => Id = token;

    public TimeZoneDbProvider(TimeZoneDbToken token, List<ApplicationTimeZone> zones)
    {
        Id = token;
        _zones = zones;
    }

    /// <summary>
    /// Метод, возрващающий временные зоны в их обновленном состоянии (текущем времени)
    /// Под капотом используется HTTP клиент для отправки запроса в сервис временных зон.
    /// <returns>
    /// <inheritdoc cref="_zones"/>
    /// </returns>
    /// </summary>
    public async Task<Result<IReadOnlyList<ApplicationTimeZone>>> ProvideTimeZones()
    {
        TimeZoneDbTimeResponseRequester requester = new(this);
        Result<TimeZoneDbTimeResponse[]> response = await requester.GetResponse();
        if (response.IsFailure)
            return Result.Failure<IReadOnlyList<ApplicationTimeZone>>(response.Error);

        InitializeNewTimeZones(response.Value);
        return _zones;
    }

    /// <summary>
    /// Инициализация временных зон в объекте <inheritdoc cref="TimeZoneDbProvider"/>
    /// <param name="response">
    /// Список    <inheritdoc cref="TimeZoneDbTimeResponse"/>
    /// </param>
    /// <returns>
    /// Success или Failure
    /// </returns>
    /// </summary>
    private Result InitializeNewTimeZones(TimeZoneDbTimeResponse[] response)
    {
        TimeZoneDbTimeAdapter adapter = new(this);
        ApplicationTimeZone[] zones = new ApplicationTimeZone[response.Length];

        for (int i = 0; i < zones.Length; i++)
        {
            TimeZoneDbTimeResponse item = response[i];
            Result<ApplicationTimeZone> zone = adapter.Adapt(item);
            if (zone.IsFailure)
                return Result.Failure(zone.Error);
            zones[i] = zone.Value;
        }

        _zones.Clear();
        _zones.AddRange(zones);
        return Result.Success();
    }
}
