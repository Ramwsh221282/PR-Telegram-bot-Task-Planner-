using CSharpFunctionalExtensions;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.RecognitionStrategies;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognizers;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade.Services;

/// <summary>
/// Сервис для сбора метаданных распознавания времени
/// </summary>
/// <param name="recognizers">Сервис распознавателей</param>
public sealed class TimeRecognitionMetadataService(TimeRecognitionRecognizersService recognizers)
{
    private readonly TimeRecognitionRecognizersService _recognizers = recognizers;

    public async Task<RecognitionMetadataCollection> CollectMetadata(
        TimeRecognitionTicket ticket
    ) =>
        ticket switch
        {
            // если ticket с нераспознанным временем, возврат пустой коллекции метаданных
            UnknownTimeRecognitionTicket => FromUnknownTimeTicket(),

            // возврат коллекции метаданных на основе периодичного времени
            PeriodicTimeRecognitionTicket t => await FromPeriodic(t),

            // возврат коллекции метаданных на основе не периодичного времени
            SingleTimeRecognitionTicket t => await FromSingle(t),
            _ => FromUnknownTimeTicket(),
        };

    private async Task<RecognitionMetadataCollection> FromPeriodic(
        PeriodicTimeRecognitionTicket ticket
    )
    {
        string input = ticket.Input;
        List<RecognitionMetadata> metadata = new(5);
        Result<Recognitions.TimeRecognition> isRelative = await IsRelative(input);
        Result<Recognitions.TimeRecognition> hasMonth = await HasMonth(input);
        Result<Recognitions.TimeRecognition> hasDayOfWeek = await HasDayOfWeek(input);
        Result<Recognitions.TimeRecognition> hasTime = await HasTime(input);
        Result<Recognitions.TimeRecognition> periodic = await IsPeriodic(input);
        if (hasTime.IsFailure && periodic.IsFailure)
            return new RecognitionMetadataCollection([]);
        if (isRelative.IsSuccess)
            metadata.Add(new RecognitionMetadata(isRelative.Value));
        if (hasMonth.IsSuccess)
            metadata.Add(new RecognitionMetadata(hasMonth.Value));
        if (hasDayOfWeek.IsSuccess)
            metadata.Add(new RecognitionMetadata(hasDayOfWeek.Value));
        if (periodic.IsSuccess)
            metadata.Add(new RecognitionMetadata(periodic.Value));
        if (hasTime.IsSuccess)
            metadata.Add(new RecognitionMetadata(hasTime.Value));
        return new RecognitionMetadataCollection(metadata);
    }

    private async Task<RecognitionMetadataCollection> FromSingle(SingleTimeRecognitionTicket ticket)
    {
        string input = ticket.Input;
        List<RecognitionMetadata> metadata = new(4);
        Result<Recognitions.TimeRecognition> isRelative = await IsRelative(input);
        Result<Recognitions.TimeRecognition> hasMonth = await HasMonth(input);
        Result<Recognitions.TimeRecognition> hasDayOfWeek = await HasDayOfWeek(input);
        Result<Recognitions.TimeRecognition> hasTime = await HasTime(input);
        if (hasTime.IsFailure)
            return new RecognitionMetadataCollection([]);
        if (isRelative.IsSuccess)
            metadata.Add(new RecognitionMetadata(isRelative.Value));
        if (hasMonth.IsSuccess)
            metadata.Add(new RecognitionMetadata(hasMonth.Value));
        if (hasDayOfWeek.IsSuccess)
            metadata.Add(new RecognitionMetadata(hasDayOfWeek.Value));
        metadata.Add(new RecognitionMetadata(hasTime.Value));
        return new RecognitionMetadataCollection(metadata);
    }

    private RecognitionMetadataCollection FromUnknownTimeTicket() => new([]);

    private async Task<Result<Recognitions.TimeRecognition>> IsRelative(string input)
    {
        RecognitionProcessor processor = new();
        processor.Strategy = _recognizers.GetStrategy<ChunkRecognitionStrategy>();
        processor.AddRecognizer(_recognizers.GetRecognizer<RelativeDateRecognizer>());
        Recognitions.TimeRecognition recognition = await processor.PerformRecognition(input);
        return Validate(recognition);
    }

    private async Task<Result<Recognitions.TimeRecognition>> HasMonth(string input)
    {
        RecognitionProcessor processor = new();
        processor.Strategy = _recognizers.GetStrategy<RawStringRecognitionStrategy>();
        processor.AddRecognizer(_recognizers.GetRecognizer<MonthRecognizer>());
        Recognitions.TimeRecognition recognition = await processor.PerformRecognition(input);
        return Validate(recognition);
    }

    private async Task<Result<Recognitions.TimeRecognition>> HasDayOfWeek(string input)
    {
        RecognitionProcessor processor = new();
        processor.Strategy = _recognizers.GetStrategy<ChunkRecognitionStrategy>();
        processor.AddRecognizer(_recognizers.GetRecognizer<DayOfWeekRecognizer>());
        Recognitions.TimeRecognition recognition = await processor.PerformRecognition(input);
        return Validate(recognition);
    }

    private async Task<Result<Recognitions.TimeRecognition>> HasTime(string input)
    {
        RecognitionProcessor processor = new();
        processor.Strategy = _recognizers.GetStrategy<RawStringRecognitionStrategy>();
        processor.AddRecognizer(_recognizers.GetRecognizer<TimeRecognizer>());
        Recognitions.TimeRecognition recognition = await processor.PerformRecognition(input);
        return Validate(recognition);
    }

    private async Task<Result<Recognitions.TimeRecognition>> IsPeriodic(string input)
    {
        RecognitionProcessor processor = new();
        processor.Strategy = _recognizers.GetStrategy<RawStringRecognitionStrategy>();
        processor.AddRecognizer(_recognizers.GetRecognizer<PeriodicTimeRecognizer>());
        Recognitions.TimeRecognition recognition = await processor.PerformRecognition(input);
        return Validate(recognition);
    }

    private Result<Recognitions.TimeRecognition> Validate(
        Recognitions.TimeRecognition recognition
    ) =>
        RecognitionValidator.IsRecognized(recognition)
            ? recognition
            : Result.Failure<Recognitions.TimeRecognition>("Time is not recognized.");
}
