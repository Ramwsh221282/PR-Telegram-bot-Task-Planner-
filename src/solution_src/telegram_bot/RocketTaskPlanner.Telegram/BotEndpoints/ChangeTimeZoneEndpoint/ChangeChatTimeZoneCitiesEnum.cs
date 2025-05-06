using PRTelegramBot.Attributes;

namespace RocketTaskPlanner.Telegram.BotEndpoints.ChangeTimeZoneEndpoint;

/// <summary>
/// Перечисление для обработки выбора временной зоны при командe /change_time_zone
/// </summary>
[InlineCommand]
public enum ChangeChatTimeZoneCitiesEnum
{
    Cancellation = 600,
    Anadyr = 601,
    Chita = 602,
    Kamchatka = 603,
    Krasnoyarsk = 604,
    Novokuzneck = 605,
    Omsk = 606,
    Srednekolysmk = 607,
    UstNera = 608,
    Yakutsk = 609,
    Astrahan = 610,
    Kirov = 611,
    Samara = 612,
    Ulyanovsk = 613,
    Barnaul = 614,
    Irkutsk = 615,
    Hadyga = 616,
    Magadan = 617,
    Novosibirsk = 618,
    Sahalin = 619,
    Tomsk = 620,
    Vladivostok = 621,
    Ekatirenburg = 622,
    Kaliningrad = 623,
    Moskwa = 624,
    Saratov = 625,
    Volgograd = 626,
}
