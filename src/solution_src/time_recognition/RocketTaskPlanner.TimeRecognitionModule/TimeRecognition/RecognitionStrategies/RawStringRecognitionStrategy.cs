using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognizers;
using RocketTaskPlanner.Utilities.StringUtilities;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.RecognitionStrategies;

public sealed class RawStringRecognitionStrategy : IRecognitionStrategy
{
    public async Task<Recognitions.TimeRecognition> Recognize(
        string input,
        ITimeRecognizer recognizer
    ) =>
        await recognizer.TryRecognize(
            input.ToLowerInvariant().CleanString().CleanStringFromPrepositionsAndConjunctions()
        );
}
