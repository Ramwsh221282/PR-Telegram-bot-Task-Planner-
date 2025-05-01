using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;
using RocketTaskPlanner.Utilities.StringUtilities;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognizers;

public sealed partial class TimeRecognizer : ITimeRecognizer
{
    public async Task<Recognitions.TimeRecognition> TryRecognize(string input)
    {
        string formatted = input.CleanStringFromPrepositionsAndConjunctions();
        Result<SpecificTimeRecognition> fromSample1 = FromRegex_Sample_1(formatted);
        if (fromSample1.IsSuccess)
            return await Task.FromResult(fromSample1.Value);
        Result<SpecificTimeRecognition> fromSample2 = FromRegex_Sample_2(formatted);
        if (fromSample2.IsSuccess)
            return await Task.FromResult(fromSample2.Value);
        Result<SpecificTimeRecognition> fromSample3 = FromRegex_Sample_3(formatted);
        if (fromSample3.IsSuccess)
            return await Task.FromResult(fromSample3.Value);
        return await Task.FromResult(new UnrecognizedTime());
    }

    private static Result<SpecificTimeRecognition> FromRegex_Sample_1(string input)
    {
        Result<string> matchedString = GetMatchedString(input, TimeRegex_Sample_1());
        if (matchedString.IsFailure)
            return Result.Failure<SpecificTimeRecognition>("Unable to determine specific time");
        string matchedStringValue = matchedString.Value;
        string formatted = matchedStringValue.KeepOnlyDigitsInString().RemoveExtraSpaces();
        string[] parts = formatted.Split(' ', StringSplitOptions.TrimEntries);
        int hour = int.Parse(parts[0]);
        int minutes = int.Parse(parts[1]);
        return new SpecificTimeRecognition(hour, minutes);
    }

    private static Result<SpecificTimeRecognition> FromRegex_Sample_2(string input)
    {
        Result<string> matchedString = GetMatchedString(input, TimeRegex_Sample_2());
        if (matchedString.IsFailure)
            return Result.Failure<SpecificTimeRecognition>("Unable to determine specific time");
        string matchedStringValue = matchedString.Value;
        string formatted = matchedStringValue.KeepOnlyDigitsInString().RemoveExtraSpaces();
        string[] parts = formatted.Split(' ', StringSplitOptions.TrimEntries);
        int hour = int.Parse(parts[0]);
        int minutes = int.Parse(parts[1]);
        return new SpecificTimeRecognition(hour, minutes);
    }

    private static Result<SpecificTimeRecognition> FromRegex_Sample_3(string input)
    {
        Result<string> matchedString = GetMatchedString(input, TimeRegex_Sample_3());
        if (matchedString.IsFailure)
            return Result.Failure<SpecificTimeRecognition>("Unable to determine specific time");
        string matchedStringValue = matchedString.Value;
        string formatted = matchedStringValue.KeepOnlyDigitsInString().RemoveExtraSpaces();
        string[] parts = formatted.Split(' ', StringSplitOptions.TrimEntries);
        int hour = int.Parse(parts[0]);
        return new SpecificTimeRecognition(hour, 0);
    }

    private static Result<string> GetMatchedString(string input, Regex regex)
    {
        Match match = regex.Match(input);
        return !match.Success || string.IsNullOrWhiteSpace(match.Groups[0].Value)
            ? Result.Failure<string>("Unable to determine specific time")
            : match.Groups[0].Value;
    }

    private const RegexOptions Options =
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant;

    [GeneratedRegex(@"(\b\d{1,2}\sч(?:асов|аса)?\s\d{1,2}\sм(?:мин|инут)?\b)", Options)]
    private static partial Regex TimeRegex_Sample_1();

    [GeneratedRegex(@"((\b\d{1,2}\s\d{1,2}\b))", Options)]
    private static partial Regex TimeRegex_Sample_2();

    [GeneratedRegex(@"(\b\d{1,2}\sч(?:асов|аса|\b))", Options)]
    private static partial Regex TimeRegex_Sample_3();

    // [GeneratedRegex(@"(\d{1,2}\sм(?:мин|инут)?)", Options)]
    // private static partial Regex TimeRegex_Sample_4();
}
