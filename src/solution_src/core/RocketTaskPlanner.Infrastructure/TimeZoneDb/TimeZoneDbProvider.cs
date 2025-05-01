using CSharpFunctionalExtensions;
using RocketTaskPlanner.Domain.ApplicationTimeContext;
using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Domain.ApplicationTimeContext.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.TimeZoneDb;

public sealed class TimeZoneDbProvider : IApplicationTimeProvider
{
    private readonly List<ApplicationTimeZone> _zones = [];
    public IApplicationTimeProviderId Id { get; } = null!;
    public IReadOnlyCollection<ApplicationTimeZone> TimeZones => _zones;

    private TimeZoneDbProvider() { } // ef core;

    public TimeZoneDbProvider(TimeZoneDbToken token) => Id = token;

    public TimeZoneDbProvider(TimeZoneDbToken token, List<ApplicationTimeZone> zones)
    {
        Id = token;
        _zones = zones;
    }

    public async Task<Result<IReadOnlyList<ApplicationTimeZone>>> ProvideTimeZones()
    {
        TimeZoneDbTimeResponseRequester requester = new(this);
        Result<TimeZoneDbTimeResponse[]> response = await requester.GetResponse();
        if (response.IsFailure)
            return Result.Failure<IReadOnlyList<ApplicationTimeZone>>(response.Error);

        InitializeNewTimeZones(response.Value);
        return _zones;
    }

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
