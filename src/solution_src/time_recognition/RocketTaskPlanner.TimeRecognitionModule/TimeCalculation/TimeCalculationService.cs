using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;

/// <summary>
/// Сервис расчета времени
/// </summary>
public sealed class TimeCalculationService
{
    /// <summary>
    /// Расчет времени от текущего времени на основе распознаваний
    /// </summary>
    /// <param name="current">Текущее время</param>
    /// <param name="result">Результат распознаваний времени</param>
    /// <returns></returns>
    public TimeCalculationItem AddOffset(TimeCalculationItem current, TimeRecognitionResult result)
    {
        RecognitionMetadataCollection metadata = result.Metadata;
        TimeCalculationItem temp = AdjustDateOffsets(current, metadata);
        temp = AdjustTimeOffset(temp, metadata);
        return temp;
    }

    /// <summary>
    /// Применение модификаторов даты к текущей дате и времени
    /// </summary>
    /// <param name="item">Текущее дата и время</param>
    /// <param name="collection">Коллекция метаданных распознавания</param>
    /// <returns>дата и время, сдвинутое от текущего, в зависимости от метаданных</returns>
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

    /// <summary>
    /// Применение модификаторов времени к текущей дате и времени
    /// </summary>
    /// <param name="item">Текущее дата и время</param>
    /// <param name="collection">Коллекция метаданных распознавания</param>
    /// <returns>дата и время, сдвинутое от текущего, в зависимости от метаданных</returns>
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
