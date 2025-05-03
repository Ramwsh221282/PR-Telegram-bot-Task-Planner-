namespace RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;

/// <summary>
/// Абстракция для добавления модификатора к времени при расчёте времени уведомления
/// </summary>
public interface ITimeModifier
{
    public TimeCalculationItem Modify(TimeCalculationItem item);
}
