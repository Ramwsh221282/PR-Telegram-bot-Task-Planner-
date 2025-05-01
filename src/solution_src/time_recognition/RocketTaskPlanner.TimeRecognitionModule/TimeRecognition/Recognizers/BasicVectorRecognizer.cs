using Build5Nines.SharpVector;
using CSharpFunctionalExtensions;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognizers;

public abstract class BasicVectorRecognizer : ITimeRecognizer
{
    private readonly BasicMemoryVectorDatabase _database = new();

    private readonly float _threshHold;

    protected BasicVectorRecognizer(float threshHold)
    {
        _threshHold = threshHold;
    }

    public abstract Task<Recognitions.TimeRecognition> TryRecognize(string input);

    protected void AddText(string text, string metadata) => _database.AddText(text, metadata);

    protected async Task<IVectorTextResult<string, string>> PerformRecognition(string input) =>
        await _database.SearchAsync(input, threshold: _threshHold);

    protected static Result<IVectorTextResultItem<string, string>> GetMostAccurateRecognition(
        IVectorTextResult<string, string> result
    ) =>
        !result.Texts.Any()
            ? Result.Failure<IVectorTextResultItem<string, string>>("No recognitions persists.")
            : Result.Success(result.Texts.First());
}
