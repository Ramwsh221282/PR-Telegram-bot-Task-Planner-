using CSharpFunctionalExtensions;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade;

public interface ITimeRecognitionFacade
{
    Task<Result<TimeRecognitionResult>> RecognizeTime(string? input);
}
