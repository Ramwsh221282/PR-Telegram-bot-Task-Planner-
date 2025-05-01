namespace RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;

public interface ITimeModifier
{
    public TimeCalculationItem Modify(TimeCalculationItem item);
}
