using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognizers;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.RecognitionStrategies;

/// <summary>
/// Стратегия для работы с текстом при распознавании времени
/// </summary>
public interface IRecognitionStrategy
{
    Task<Recognitions.TimeRecognition> Recognize(string input, ITimeRecognizer recognizer);
}
