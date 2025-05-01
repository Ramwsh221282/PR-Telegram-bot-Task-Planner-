using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognizers;
using RocketTaskPlanner.Utilities.StringUtilities;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.RecognitionStrategies;

public sealed class ChunkRecognitionStrategy : IRecognitionStrategy
{
    public async Task<Recognitions.TimeRecognition> Recognize(
        string input,
        ITimeRecognizer recognizer
    )
    {
        string[] words = SplitIntoWords(
            input.ToLowerInvariant().CleanString().CleanStringFromPrepositionsAndConjunctions()
        );

        foreach (string word in words)
        {
            Recognitions.TimeRecognition result = await recognizer.TryRecognize(word);
            bool shouldStop = ShouldStop(result);
            if (shouldStop)
                return result;
        }

        return new UnrecognizedTime();
    }

    private static string[] SplitIntoWords(string input) =>
        input.Split(' ', StringSplitOptions.TrimEntries);

    private static bool ShouldStop(Recognitions.TimeRecognition recognition) =>
        recognition switch
        {
            UnrecognizedTime => false,
            _ => true,
        };
}
