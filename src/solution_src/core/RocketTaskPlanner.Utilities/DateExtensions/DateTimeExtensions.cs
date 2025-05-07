namespace RocketTaskPlanner.Utilities.DateExtensions;

/// <summary>
/// Utility Extension методы для работы с DateTime
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Форматированная строка DateTime
    /// <param name="date">
    ///     <inheritdoc cref="DateTime"/>
    /// </param>
    /// <returns>Строка с даты в формате Часы:Минуты:Секунды НомерДня/НомерМесяца/НомерГода</returns>
    /// </summary>
    public static string AsString(this DateTime date) => date.ToString("HH:mm:ss dd/MM/yyyy");
}
