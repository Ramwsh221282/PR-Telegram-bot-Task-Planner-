namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;

/// <summary>
/// Результат распознавания даты и времени
/// </summary>
/// <param name="Metadata">Коллекция метаданных даты и времени</param>
/// <param name="IsPeriodic">Периодичное время или нет</param>
public record TimeRecognitionResult(RecognitionMetadataCollection Metadata, bool IsPeriodic);
