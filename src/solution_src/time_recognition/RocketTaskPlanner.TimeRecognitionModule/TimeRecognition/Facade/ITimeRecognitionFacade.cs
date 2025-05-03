using CSharpFunctionalExtensions;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade;

/// <summary>
/// Фасадный класс для распознавания времени
/// </summary>
public interface ITimeRecognitionFacade
{
    /// <summary>
    /// Метод распознавания времени
    /// </summary>
    /// <param name="input">Текст</param>
    /// <returns>Результат распознавания Success если есть распознавание, в противном случае Failure</returns>
    Task<Result<TimeRecognitionResult>> RecognizeTime(string? input);
}
