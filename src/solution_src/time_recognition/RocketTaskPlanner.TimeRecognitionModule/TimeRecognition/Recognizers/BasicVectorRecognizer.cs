using Build5Nines.SharpVector;
using CSharpFunctionalExtensions;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognizers;

/// <summary>
/// Базовый класс распознавателей, использующие векторный поиск из SharpVector.
/// </summary>
public abstract class BasicVectorRecognizer : ITimeRecognizer
{
    private readonly BasicMemoryVectorDatabase _database = new();

    /// <summary>
    /// Порог точности при распознавании, результаты распознавания ниже значения будут игнорироваться в IVectorTextResult
    /// </summary>
    private readonly float _threshHold;

    /// <summary>
    /// Конструктор класса распознавателей, использующий векторный поиск в SharpVector
    /// </summary>
    /// <param name="threshHold">Порог допустимости при распознавании. Ниже этого порога результаты будут игнорироваться</param>
    protected BasicVectorRecognizer(float threshHold) => _threshHold = threshHold;

    /// <summary>
    /// Попытаться распознать время в строке.
    /// </summary>
    /// <param name="input">Входной текст</param>
    /// <returns>Результат распознавания</returns>
    public abstract Task<Recognitions.TimeRecognition> TryRecognize(string input);

    /// <summary>
    /// Добавить текст в In Memory векторную БД.
    /// </summary>
    /// <param name="text">Текст для распознавания</param>
    /// <param name="metadata">Метка. Если будет результат распознавания, то в результате распознавания сохранится эта метка.</param>
    protected void AddText(string text, string metadata) => _database.AddText(text, metadata);

    /// <summary>
    /// Произвести распознавание времени в тексте
    /// </summary>
    /// <param name="input">Текст</param>
    /// <returns>Результаты векторного распознавания</returns>
    protected async Task<IVectorTextResult<string, string>> PerformRecognition(string input) =>
        await _database.SearchAsync(input, threshold: _threshHold);

    /// <summary>
    /// Получение результата распознавания с наибольшей точностью, учитывая порог threshold из результатов распознавания
    /// </summary>
    /// <param name="result"></param>
    /// <returns>Success, если результат распознавания с наибольшей точностью существует, в противном случае Failure</returns>
    protected static Result<IVectorTextResultItem<string, string>> GetMostAccurateRecognition(
        IVectorTextResult<string, string> result
    ) =>
        !result.Texts.Any()
            ? Result.Failure<IVectorTextResultItem<string, string>>("No recognitions persists.")
            : Result.Success(result.Texts.First());
}
