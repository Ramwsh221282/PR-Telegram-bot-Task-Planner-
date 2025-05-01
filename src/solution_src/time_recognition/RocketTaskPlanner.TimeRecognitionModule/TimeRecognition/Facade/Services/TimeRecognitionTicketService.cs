using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.RecognitionStrategies;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognizers;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade.Services;

public sealed class TimeRecognitionTicketService(TimeRecognitionRecognizersService recognizers)
{
    private readonly TimeRecognitionRecognizersService _recognizers = recognizers;

    public async Task<TimeRecognitionTicket> CreateRecognitionTicket(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return new UnknownTimeRecognitionTicket();

        bool hasTime = await HasTime(message);
        bool isPeriodic = await IsPeriodic(message);

        return hasTime switch
        {
            false when !isPeriodic => new UnknownTimeRecognitionTicket(),
            false when isPeriodic => new PeriodicTimeRecognitionTicket(message),
            true when !isPeriodic => new SingleTimeRecognitionTicket(message),
            true when isPeriodic => new PeriodicTimeRecognitionTicket(message),
            _ => new UnknownTimeRecognitionTicket(),
        };
    }

    private async Task<bool> HasTime(string input)
    {
        IRecognitionStrategy strategy = _recognizers.GetStrategy<RawStringRecognitionStrategy>();
        ITimeRecognizer recognizer = _recognizers.GetRecognizer<TimeRecognizer>();
        RecognitionProcessor processor = new() { Strategy = strategy };
        processor.AddRecognizer(recognizer);
        Recognitions.TimeRecognition recognition = await processor.PerformRecognition(input);
        return RecognitionValidator.IsRecognized(recognition);
    }

    private async Task<bool> IsPeriodic(string input)
    {
        IRecognitionStrategy strategy = _recognizers.GetStrategy<RawStringRecognitionStrategy>();
        ITimeRecognizer recognizer = _recognizers.GetRecognizer<PeriodicTimeRecognizer>();
        RecognitionProcessor processor = new() { Strategy = strategy };
        processor.AddRecognizer(recognizer);
        Recognitions.TimeRecognition recognition = await processor.PerformRecognition(input);
        return RecognitionValidator.IsRecognized(recognition);
    }
}
