namespace RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;

/// <summary>
/// Абстракция для расчета времени уведомления
/// </summary>
public interface ITimeCalculation : ITimeModifier
{
    public TimeCalculationItem Calculate(TimeCalculationItem item);
}
