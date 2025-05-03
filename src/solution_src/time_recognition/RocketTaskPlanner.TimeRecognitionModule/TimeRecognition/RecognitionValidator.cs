using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition;

/// <summary>
/// Utility класс для проверки на распознанность
/// </summary>
public static class RecognitionValidator
{
    public static bool IsRecognized(Recognitions.TimeRecognition recognition) =>
        recognition switch
        {
            UnrecognizedTime => false,
            _ => true,
        };
}
