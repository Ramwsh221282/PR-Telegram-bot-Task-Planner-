using System.Collections;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;

/// <summary>
/// Метаданные распознавания
/// </summary>
/// <param name="Recognition">Результат распознавания</param>
public record RecognitionMetadata(TimeRecognition Recognition);

/// <summary>
/// Коллекция метаданных распознавания
/// </summary>
public record RecognitionMetadataCollection : IEnumerable<RecognitionMetadata>
{
    private readonly List<RecognitionMetadata> _metadata;

    public RecognitionMetadataCollection(List<RecognitionMetadata> metadata) =>
        _metadata = metadata;

    public int Count => _metadata.Count;

    public IEnumerator<RecognitionMetadata> GetEnumerator() => _metadata.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
