using CSharpFunctionalExtensions;
using RocketTaskPlanner.Domain.ApplicationTimeContext.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.TimeZoneDb;

public sealed record TimeZoneDbToken : IApplicationTimeProviderId
{
    public string Id { get; }

    private TimeZoneDbToken() => Id = string.Empty;

    private TimeZoneDbToken(string id) => Id = id;

    public static Result<TimeZoneDbToken> Create(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return Result.Failure<TimeZoneDbToken>("Time Zone Db Api Token был пустым.");

        if (!IsValid(id))
        {
            string errorMessage = """
                Этот текст не похож на токен из Time Zone Db.
                Time Zone Db должен предоставить токен со случайной последовательностью символов в верхнем регистре.
                """;
            return Result.Failure<TimeZoneDbToken>(errorMessage);
        }

        return new TimeZoneDbToken(id);
    }

    private static bool IsValid(string token)
    {
        foreach (var character in token)
        {
            if (char.IsDigit(character))
                continue;
            if (char.IsUpper(character))
                continue;
            return false;
        }
        return true;
    }
}
