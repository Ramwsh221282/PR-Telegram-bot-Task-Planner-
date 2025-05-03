using RocketTaskPlanner.Utilities.UnixTimeUtilities;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;

/// <summary>
/// Класс для хранения данных о времени.
/// </summary>
public sealed class TimeCalculationItem
{
    public long TimeStamp { get; }
    public DateTime CalculationDateTime { get; }
    public bool IsPeriodic { get; }

    public TimeCalculationItem(long seconds, DateTime calculationDateTime, bool isPeriodic)
    {
        TimeStamp = seconds;
        CalculationDateTime = calculationDateTime;
        IsPeriodic = isPeriodic;
    }

    public TimeCalculationItem(TimeCalculationItem other)
        : this(other.TimeStamp, other.TimeStamp.FromUnixTimeSeconds(), other.IsPeriodic) { }

    public TimeCalculationItem(TimeCalculationItem other, long seconds)
        : this(other)
    {
        TimeStamp += seconds;
        CalculationDateTime = TimeStamp.FromUnixTimeSeconds();
    }

    public TimeCalculationItem(TimeCalculationItem other, DateTime calculated)
        : this(other)
    {
        TimeStamp = calculated.ToUnixTimeSeconds();
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
