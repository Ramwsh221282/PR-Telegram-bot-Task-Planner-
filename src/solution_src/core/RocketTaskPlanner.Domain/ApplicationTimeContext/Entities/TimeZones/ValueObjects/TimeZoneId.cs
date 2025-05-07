namespace RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones.ValueObjects;

/// <summary>
/// ID временной зоны
/// </summary>
public sealed record TimeZoneId
{
    /// <summary>
    /// ID
    /// </summary>
    public string Id { get; }

    private TimeZoneId() => Id = string.Empty;

    private TimeZoneId(string id) => Id = id;

    public static Result<TimeZoneId> Create(string? id) =>
        string.IsNullOrWhiteSpace(id)
            ? Result.Failure<TimeZoneId>("Ключ временной зоны был пустым.")
            : new TimeZoneId(id);
}
