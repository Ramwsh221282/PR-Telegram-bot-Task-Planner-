using CSharpFunctionalExtensions;
using RocketTaskPlanner.Domain.ApplicationTimeContext;
using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.TimeZoneDb;

/// <summary>
/// Класс адаптер временных зон из Time Zone Db сервиса, в модель временных зон приложения
/// <param name="provider">
///     <inheritdoc cref="TimeZoneDbProvider"/>
/// </param>
/// </summary>
public sealed class TimeZoneDbTimeAdapter(IApplicationTimeProvider provider)
{
    /// <summary>
    /// Список для русификации временных зон.
    /// </summary>
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
        { @"Asia/Yekaterinburg", "Екатеринбург" },
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

    /// <summary>
    /// Создание модели временной зоны
    ///     <inheritdoc cref="ApplicationTimeZone"/>
    /// из
    ///     <inheritdoc cref="TimeZoneDbTimeResponse"/>
    /// <param name="response">
    ///     <inheritdoc cref="TimeZoneDbTimeResponse"/>
    /// </param>
    /// <returns>
    ///     <inheritdoc cref="ApplicationTimeZone"/>
    /// </returns>
    /// </summary>
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

    /// <summary>
    /// Русификация временной зоны
    /// </summary>
    /// <param name="zoneName">Нерусифицированная строка временной зоны</param>
    /// <returns>Русифицированная строка временной зоны</returns>
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
