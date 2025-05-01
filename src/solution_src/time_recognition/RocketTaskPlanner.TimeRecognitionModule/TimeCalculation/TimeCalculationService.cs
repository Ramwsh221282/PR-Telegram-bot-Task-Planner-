using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;

public sealed class TimeCalculationService
{
    public TimeCalculationItem AddOffset(TimeCalculationItem current, TimeRecognitionResult result)
    {
        RecognitionMetadataCollection metadata = result.Metadata;
        TimeCalculationItem temp = AdjustDateOffsets(current, metadata);
        temp = AdjustTimeOffset(temp, metadata);
        return temp;
    }

    private static TimeCalculationItem AdjustDateOffsets(
        TimeCalculationItem item,
        RecognitionMetadataCollection collection
    )
    {
        RecognitionMetadataCollection filetred = new RecognitionMetadataCollection(
            [.. collection.Where(i => i.Recognition.GetType() != typeof(SpecificTimeRecognition))]
        );
        return filetred.Aggregate(
            item,
            (current, metadata) => metadata.Recognition.Modify(current)
        );
    }

    private static TimeCalculationItem AdjustTimeOffset(
        TimeCalculationItem item,
        RecognitionMetadataCollection collection
    )
    {
        RecognitionMetadata? timeRecognition = collection.FirstOrDefault(i =>
            i.Recognition is SpecificTimeRecognition
        );
        return timeRecognition == null ? item : timeRecognition.Recognition.Modify(item);
    }
}
