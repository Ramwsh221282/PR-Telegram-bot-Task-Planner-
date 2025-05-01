using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognizers;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.RecognitionStrategies;

public interface IRecognitionStrategy
{
    Task<Recognitions.TimeRecognition> Recognize(string input, ITimeRecognizer recognizer);
}
