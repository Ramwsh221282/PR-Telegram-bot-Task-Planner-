using CSharpFunctionalExtensions;
using RocketTaskPlanner.Domain.ApplicationTimeContext;
using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.TimeZoneDb;

public sealed class TimeZoneDbTimeAdapter(IApplicationTimeProvider provider)
{
    private static readonly Dictionary<string, string> _zoneNames = new Dictionary<string, string>()
    {
        { @"Asia/Anadyr", "Анадырь" },
        { @"Asia/Barnaul", "Барнаул" },
        { @"Asia/Chita", "Чита" },
        { @"Asia/Irkutsk", "Иркутск" },
        { @"Asia/Kamchatka", "Камчатка" },
        { @"Asia/Khandyga", "Хадыга" },
        { @"Asia/Krasnoyarsk", "Красноярск" },
        { @"Asia/Magadan", "Магадан" },
        { @"Asia/Novokuznetsk", "Новокузнецк" },
        { @"Asia/Novosibirsk", "Новосибирск" },
        { @"Asia/Omsk", "Омск" },
        { @"Asia/Sakhalin", "Сахалин" },
        { @"Asia/Srednekolymsk", "Среднеколымск" },
        { @"Asia/Tomsk", "Томск" },
        { @"Asia/Ust-Nera", "Усть-Нера" },
        { @"Asia/Vladivostok", "Владивосток" },
        { @"Asia/Yakutsk", "Якутск" },
        { @"Asia/Yekaterinburg", "Екатиренбург" },
        { @"Europe/Astrakhan", "Астрахань" },
        { @"Europe/Kaliningrad", "Калининград" },
        { @"Europe/Kirov", "Киров" },
        { @"Europe/Moscow", "Москва" },
        { @"Europe/Samara", "Самара" },
        { @"Europe/Saratov", "Саратов" },
        { @"Europe/Ulyanovsk", "Ульяновск" },
        { @"Europe/Volgograd", "Волгоград" },
    };

    private readonly IApplicationTimeProvider _provider = provider;

    public Result<ApplicationTimeZone> Adapt(TimeZoneDbTimeResponse response)
    {
        Result<TimeZoneId> id = TimeZoneId.Create(response.ZoneName);
        if (id.IsFailure)
            return Result.Failure<ApplicationTimeZone>(id.Error);

        Result<string> displayName = GetDisplayName(id.Value.Id);
        if (displayName.IsFailure)
            return Result.Failure<ApplicationTimeZone>(displayName.Error);

        Result<TimeZoneName> name = TimeZoneName.Create(displayName.Value);
        if (name.IsFailure)
            return Result.Failure<ApplicationTimeZone>(name.Error);

        Result<TimeZoneTimeInfo> time = TimeZoneTimeInfo.Create(response.TimeStamp);
        if (time.IsFailure)
            return Result.Failure<ApplicationTimeZone>(time.Error);

        ApplicationTimeZone zone = new ApplicationTimeZone(
            id.Value,
            name.Value,
            time.Value,
            _provider
        );

        return zone;
    }

    private static Result<string> GetDisplayName(string zoneName)
    {
        if (_zoneNames.TryGetValue(zoneName, out string? displayName))
        {
            return string.IsNullOrWhiteSpace(displayName)
                ? Result.Failure<string>($"Временная зона: {zoneName} не поддерживается.")
                : (Result<string>)displayName;
        }

        return Result.Failure<string>($"Временная зона: {zoneName} не поддерживается.");
    }
}
