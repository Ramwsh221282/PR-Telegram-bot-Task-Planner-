using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.RecognitionStrategies;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognizers;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition;

/// <summary>
/// Класс для выполнения распознаваний времени с какой-либо стратегией распознавания и списком распознавателей
/// </summary>
public sealed class RecognitionProcessor
{
    /// <summary>
    /// Список распознавателей
    /// </summary>
    private readonly List<ITimeRecognizer> _recognizers = [];

    /// <summary>
    /// Стратегия работы с текстовой строкой при распознавании
    /// </summary>
    public IRecognitionStrategy? Strategy { get; set; }

    /// <summary>
    /// Добавление распознавателя в список распознавателей
    /// </summary>
    /// <param name="recognizer">Распознаватель</param>
    public void AddRecognizer(ITimeRecognizer recognizer) => _recognizers.Add(recognizer);

    /// <summary>
    /// Выполнение распознавания
    /// </summary>
    /// <param name="input">Входной текст</param>
    /// <returns>Информация о распознавании</returns>
    public async Task<Recognitions.TimeRecognition> PerformRecognition(string input)
    {
        if (Strategy == null)
            return new UnrecognizedTime();
        foreach (ITimeRecognizer recognizer in _recognizers)
        {
            Recognitions.TimeRecognition recognition = await Strategy.Recognize(input, recognizer);
            if (ShouldStop(recognition))
                return recognition;
        }

        return new UnrecognizedTime();
    }

    /// <summary>
    /// Метод определения нужно ли распознавать что-либо в результате распознавания
    /// </summary>
    /// <param name="recognition">Результат распознавания</param>
    /// <returns>True если результат распознавания не Unrecognized, False если Unrecognized</returns>
    private static bool ShouldStop(Recognitions.TimeRecognition recognition) =>
        recognition switch
        {
            UnrecognizedTime => false,
            _ => true,
        };
}
