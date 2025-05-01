namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognizers;

public interface ITimeRecognizer
{
    Task<Recognitions.TimeRecognition> TryRecognize(string input);
}
