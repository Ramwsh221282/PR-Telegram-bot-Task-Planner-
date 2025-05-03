using PRTelegramBot.Attributes;

namespace RocketTaskPlanner.Telegram.BotEndpoints.AddThisChatEndpoint.Handlers.AddGeneralChat;

/// <summary>
/// Перечисление для хранения индексов кнопок меню выбора временной зоны
/// </summary>
[InlineCommand]
public enum AddGeneralChatCitiesEnum
{
    Cancellation = 500,
    Anadyr = 501,
    Chita = 502,
    Kamchatka = 503,
    Krasnoyarsk = 504,
    Novokuzneck = 505,
    Omsk = 506,
    Srednekolysmk = 507,
    UstNera = 508,
    Yakutsk = 509,
    Astrahan = 510,
    Kirov = 511,
    Samara = 512,
    Ulyanovsk = 513,
    Barnaul = 514,
    Irkutsk = 515,
    Hadyga = 516,
    Magadan = 517,
    Novosibirsk = 518,
    Sahalin = 519,
    Tomsk = 520,
    Vladivostok = 521,
    Ekatirenburg = 522,
    Kaliningrad = 523,
    Moskwa = 524,
    Saratov = 525,
    Volgograd = 526,
}
