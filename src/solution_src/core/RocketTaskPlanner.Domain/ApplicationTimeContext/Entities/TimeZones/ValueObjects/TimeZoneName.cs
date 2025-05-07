namespace RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones.ValueObjects;

/// <summary>
/// Название временной зоны
/// </summary>
public sealed record TimeZoneName
{
    /// <summary>
    /// Название временной зоны
    /// </summary>
    public string Name { get; }

    private TimeZoneName() => Name = string.Empty;

    private TimeZoneName(string name) => Name = name;

    public static Result<TimeZoneName> Create(string? name) =>
        string.IsNullOrWhiteSpace(name)
            ? Result.Failure<TimeZoneName>("Название временной зоны было пустым")
            : new TimeZoneName(name);
}
