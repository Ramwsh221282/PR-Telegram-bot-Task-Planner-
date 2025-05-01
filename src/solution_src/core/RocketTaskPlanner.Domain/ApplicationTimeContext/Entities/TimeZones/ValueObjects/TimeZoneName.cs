namespace RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones.ValueObjects;

public sealed record TimeZoneName
{
    public string Name { get; }

    private TimeZoneName() => Name = string.Empty;

    private TimeZoneName(string name) => Name = name;

    public static Result<TimeZoneName> Create(string? name) =>
        string.IsNullOrWhiteSpace(name)
            ? Result.Failure<TimeZoneName>("Название временной зоны было пустым")
            : new TimeZoneName(name);
}
