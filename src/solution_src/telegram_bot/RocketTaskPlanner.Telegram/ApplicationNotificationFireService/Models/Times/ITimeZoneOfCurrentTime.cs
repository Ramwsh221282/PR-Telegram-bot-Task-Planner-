using RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Receivers;

namespace RocketTaskPlanner.Telegram.ApplicationNotificationFireService.Models.Times;

/// <summary>
/// Временная зона текущего времени.
/// </summary>
public interface ITimeZoneOfCurrentTime
{
    /// <summary>
    /// Название временной зоны
    /// </summary>
    /// <returns>Название временной зоны</returns>
    public string ZoneName();

    /// <summary>
    /// Дата временной зоны (текущая)
    /// </summary>
    /// <returns>Дата временной зоны</returns>
    public DateTime DateTime();

    /// <summary>
    /// ID временной зоны
    /// </summary>
    /// <returns>ID временной зоны</returns>
    public string Id();

    /// <summary>
    /// Список получателей уведомлений этой временной зоны
    /// </summary>
    /// <returns></returns>
    Task<GeneralChatReceiverOfCurrentTimeZone[]> Receivers();

    /// <summary>
    /// Создание экземпляра текущей временной зоны с получателями.
    /// </summary>
    /// <param name="receivers">Получатели текущей временной зоны</param>
    /// <returns>Экземпляр временной зоны с получателями</returns>
    ITimeZoneOfCurrentTime WithReceivers(GeneralChatReceiverOfCurrentTimeZone[] receivers);
}
