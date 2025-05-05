using RocketTaskPlanner.Utilities.UnixTimeUtilities;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;

/// <summary>
/// Класс для хранения данных о времени.
/// </summary>
public sealed class TimeCalculationItem
{
    public DateTime CalculationDateTime { get; }
    public bool IsPeriodic { get; }

    public TimeCalculationItem(DateTime calculationDateTime, bool isPeriodic)
    {
        CalculationDateTime = calculationDateTime;
        IsPeriodic = isPeriodic;
    }

    public TimeCalculationItem(TimeCalculationItem other)
        : this(other.CalculationDateTime, other.IsPeriodic) { }

    public TimeCalculationItem(TimeCalculationItem other, DateTime calculated)
        : this(other)
    {
        CalculationDateTime = calculated;
    }

    public override string ToString()
    {
        return $"""
            Информация о времени приложения:
            Время: {DateString}
            """;
    }

    public string DateString => CalculationDateTime.ToString("HH:mm:ss dd/MM/yyyy");
}
