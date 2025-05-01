using System.Diagnostics;
using RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;

public abstract record TimeRecognition : ITimeCalculation
{
    public abstract TimeCalculationItem Calculate(TimeCalculationItem item);

    public abstract TimeCalculationItem Modify(TimeCalculationItem item);
}

public record UnrecognizedTime : TimeRecognition
{
    public override TimeCalculationItem Calculate(TimeCalculationItem item) => item;

    public override TimeCalculationItem Modify(TimeCalculationItem item) => item;
}

public sealed record DayOfWeekRecognition(string DayOfWeek) : TimeRecognition
{
    public static readonly DayOfWeekRecognition Monday = new("Понедельник");
    public static readonly DayOfWeekRecognition Tuesday = new("Вторник");
    public static readonly DayOfWeekRecognition Wednesday = new("Среда");
    public static readonly DayOfWeekRecognition Thursday = new("Четверг");
    public static readonly DayOfWeekRecognition Friday = new("Пятница");
    public static readonly DayOfWeekRecognition Saturday = new("Суббота");
    public static readonly DayOfWeekRecognition Sunday = new("Воскресенье");

    public override TimeCalculationItem Modify(TimeCalculationItem item) => Calculate(item);

    public override TimeCalculationItem Calculate(TimeCalculationItem item)
    {
        int dayOfWeekNumber = GetDayOfWeekNumber();
        int currentDayOfWeek = (int)item.CalculationDateTime.DayOfWeek;
        int daysUntilRequested = dayOfWeekNumber - currentDayOfWeek;
        if (daysUntilRequested < 0)
            daysUntilRequested += 7;
        DateTime dateWithAddedDays = item.CalculationDateTime.Date.AddDays(daysUntilRequested);
        return new TimeCalculationItem(item, dateWithAddedDays);
    }

    private int GetDayOfWeekNumber()
    {
        return DayOfWeek switch
        {
            "Понедельник" => 1,
            "Вторник" => 2,
            "Среда" => 3,
            "Четверг" => 4,
            "Пятница" => 5,
            "Суббота" => 6,
            "Воскресенье" => 0,
            _ => throw new UnreachableException(),
        };
    }
}

public sealed record MonthRecognition(int Month, int MonthDay) : TimeRecognition
{
    public override TimeCalculationItem Modify(TimeCalculationItem item) => Calculate(item);

    public override TimeCalculationItem Calculate(TimeCalculationItem item)
    {
        DateTime currentDate = item.CalculationDateTime;
        int year = currentDate.Year;
        DateTime newDate = new(year, Month, MonthDay);
        if (newDate < currentDate)
            newDate = newDate.AddYears(1);
        return new TimeCalculationItem(item, newDate);
    }
}

public abstract record PeriodicRecognition : TimeRecognition;

public sealed record PeriodicWeekDayRecognition(DayOfWeekRecognition DayOfWeek)
    : PeriodicRecognition
{
    public override TimeCalculationItem Calculate(TimeCalculationItem calculation) =>
        DayOfWeek.Calculate(calculation);

    public override TimeCalculationItem Modify(TimeCalculationItem time) => Calculate(time);
}

public sealed record PeriodicEveryDayRecognition(RelativeRecognition Relative) : PeriodicRecognition
{
    public override TimeCalculationItem Calculate(TimeCalculationItem calculation) =>
        Relative.Calculate(calculation);

    public override TimeCalculationItem Modify(TimeCalculationItem time) => Calculate(time);
}

public sealed record PeriodicEveryHourRecognition(SpecificTimeRecognition Specific)
    : PeriodicRecognition
{
    public override TimeCalculationItem Calculate(TimeCalculationItem calculation)
    {
        DateTime current = calculation.CalculationDateTime;
        DateTime shifted = current.AddHours(Specific.Hours);
        return new TimeCalculationItem(calculation, shifted);
    }

    public override TimeCalculationItem Modify(TimeCalculationItem time) => Calculate(time);
}

public sealed record PeriodicEveryMinuteRecognition(SpecificTimeRecognition Specific)
    : PeriodicRecognition
{
    public override TimeCalculationItem Calculate(TimeCalculationItem calculation)
    {
        DateTime current = calculation.CalculationDateTime;
        DateTime shifted = current.AddMinutes(Specific.Minutes);
        return new TimeCalculationItem(calculation, shifted);
    }

    public override TimeCalculationItem Modify(TimeCalculationItem time) => Calculate(time);
}

public sealed record RelativeRecognition(int DaysOffset) : TimeRecognition
{
    public override TimeCalculationItem Calculate(TimeCalculationItem calculation)
    {
        DateTime calculated = calculation.CalculationDateTime.AddDays(DaysOffset);
        DateTime shiftedDateTime = new(
            calculated.Year,
            calculated.Month,
            calculated.Day,
            calculated.Hour,
            calculated.Minute,
            0,
            calculated.Kind
        );
        return new TimeCalculationItem(calculation, shiftedDateTime);
    }

    public override TimeCalculationItem Modify(TimeCalculationItem time) => Calculate(time);
}

public sealed record SpecificTimeRecognition(int Hours, int Minutes) : TimeRecognition
{
    public override TimeCalculationItem Calculate(TimeCalculationItem calculation)
    {
        DateTime calculated = calculation.CalculationDateTime;
        DateTime shifted = new(
            calculated.Year,
            calculated.Month,
            calculated.Day,
            Hours,
            Minutes,
            0,
            calculated.Kind
        );
        return new TimeCalculationItem(calculation, shifted);
    }

    public override TimeCalculationItem Modify(TimeCalculationItem time) => Calculate(time);
}
