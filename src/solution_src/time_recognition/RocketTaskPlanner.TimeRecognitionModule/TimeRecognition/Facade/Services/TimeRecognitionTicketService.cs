using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.RecognitionStrategies;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognizers;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade.Services;

/// <summary>
/// Сервис для определения периодичное время или нет
/// </summary>
/// <param name="recognizers">Сервис с распознавателями</param>
public sealed class TimeRecognitionTicketService(TimeRecognitionRecognizersService recognizers)
{
    private readonly TimeRecognitionRecognizersService _recognizers = recognizers;

    /// <summary>
    /// Метод для конкретизации типа времени (периодичное или нет)
    /// </summary>
    /// <param name="message">Текст</param>
    /// <returns>Dto модель для дальнейшего распознавания</returns>
    public async Task<TimeRecognitionTicket> CreateRecognitionTicket(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return new UnknownTimeRecognitionTicket();

        bool hasTime = await HasTime(message);
        bool isPeriodic = await IsPeriodic(message);

        return hasTime switch
        {
            // если время не распознано и задача не периодичная, то нет распознаваний
            false when !isPeriodic => new UnknownTimeRecognitionTicket(),

            // если время не распознано, но задача периодичная, то создание ticket о периодичном распознавании
            false when isPeriodic => new PeriodicTimeRecognitionTicket(message),

            // если время распознано, но задача не периодичная, то создание ticket о непериодичном распознавании
            true when !isPeriodic => new SingleTimeRecognitionTicket(message),

            // если время распознано, и периодично, то создание ticket о периодичном распознавании
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
