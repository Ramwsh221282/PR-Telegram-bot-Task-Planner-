namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognizers;

/// <summary>
/// Интерфейс для распознавателей времени в тексте
/// </summary>
public interface ITimeRecognizer
{
    Task<Recognitions.TimeRecognition> TryRecognize(string input);
}
