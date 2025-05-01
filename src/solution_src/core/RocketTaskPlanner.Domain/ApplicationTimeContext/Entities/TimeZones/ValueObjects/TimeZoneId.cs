namespace RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones.ValueObjects;

public sealed record TimeZoneId
{
    public string Id { get; }

    private TimeZoneId() => Id = string.Empty;

    private TimeZoneId(string id) => Id = id;

    public static Result<TimeZoneId> Create(string? id) =>
        string.IsNullOrWhiteSpace(id)
            ? Result.Failure<TimeZoneId>("Ключ временной зоны был пустым.")
            : new TimeZoneId(id);
}
