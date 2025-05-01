using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition;

public sealed class RecognitionValidator
{
    public static bool IsRecognized(Recognitions.TimeRecognition recognition) =>
        recognition switch
        {
            UnrecognizedTime => false,
            _ => true,
        };

    public static bool CanProcessTicket(TimeRecognitionTicket ticket) =>
        ticket switch
        {
            UnknownTimeRecognitionTicket => false,
            _ => true,
        };
}
