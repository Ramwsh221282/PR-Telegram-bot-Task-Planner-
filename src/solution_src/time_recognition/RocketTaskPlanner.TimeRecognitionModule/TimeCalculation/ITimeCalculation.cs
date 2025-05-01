namespace RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;

public interface ITimeCalculation : ITimeModifier
{
    public TimeCalculationItem Calculate(TimeCalculationItem item);
}
