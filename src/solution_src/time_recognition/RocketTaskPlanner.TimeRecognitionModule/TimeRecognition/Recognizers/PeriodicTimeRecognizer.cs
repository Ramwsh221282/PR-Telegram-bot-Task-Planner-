using System.Diagnostics;
using System.Text.RegularExpressions;
using Build5Nines.SharpVector;
using CSharpFunctionalExtensions;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognizers;

public sealed partial class PeriodicTimeRecognizer : BasicVectorRecognizer
{
    public PeriodicTimeRecognizer()
        : base(0.8f)
    {
        AddText("каждый понедельник", "every_monday");
        AddText("каждый вторник", "every_tuesday");
        AddText("каждый четверг", "every_thursday");
        AddText("каждую пятницу", "every_friday");
        AddText("каждую субботу", "every_saturday");
        AddText("каждую среду", "every_wednesday");
        AddText("каждое воскресенье", "every_sunday");
        AddText("каждый день", "every_day");
    }

    public override async Task<Recognitions.TimeRecognition> TryRecognize(string input)
    {
        IVectorTextResult<string, string> result = await PerformRecognition(input);
        Result<IVectorTextResultItem<string, string>> mostAccurate = GetMostAccurateRecognition(
            result
        );
        if (mostAccurate.IsSuccess)
            return FromMetadata(mostAccurate.Value.Metadata!);
        Result<PeriodicEveryDayRecognition> fromRegexEveryDay = EveryDayFromRegex(input);
        if (fromRegexEveryDay.IsSuccess)
            return fromRegexEveryDay.Value;
        Result<PeriodicEveryMinuteRecognition> fromRegexEveryMinute = EveryMinuteFromRegex(input);
        if (fromRegexEveryMinute.IsSuccess)
            return fromRegexEveryMinute.Value;
        Result<PeriodicEveryHourRecognition> fromRegexEveryHour = FromEveryHourRegex(input);
        if (fromRegexEveryHour.IsSuccess)
            return fromRegexEveryHour.Value;
        Result<PeriodicEveryHourRecognition> fromRegexEveryHourUnspecified =
            FromEveryHourUnspecifiedRegex(input);
        if (fromRegexEveryHourUnspecified.IsSuccess)
            return fromRegexEveryHourUnspecified.Value;
        Result<PeriodicEveryMinuteRecognition> fromRegexEveryMinuteUnspecified =
            FromEveryMinuteUnspecifiedRegex(input);
        if (fromRegexEveryMinuteUnspecified.IsSuccess)
            return fromRegexEveryMinuteUnspecified.Value;
        return new UnrecognizedTime();
    }

    private static Result<PeriodicEveryDayRecognition> EveryDayFromRegex(string input)
    {
        Result<string> matchedString = FromRegexMatch(EveryDayRegex(), input);
        return matchedString.IsSuccess
            ? FromEveryDayRecognition(matchedString.Value)
            : Result.Failure<PeriodicEveryDayRecognition>(
                "Unable to determine every day recognition"
            );
    }

    private static Result<PeriodicEveryMinuteRecognition> EveryMinuteFromRegex(string input)
    {
        Result<string> matchedString = FromRegexMatch(EveryMinuteRegex(), input);
        return matchedString.IsSuccess
            ? FromEveryMinuteRecognition(matchedString.Value)
            : Result.Failure<PeriodicEveryMinuteRecognition>(
                "Unable to determine every minute recognition"
            );
    }

    private static Result<PeriodicEveryHourRecognition> FromEveryHourRegex(string input)
    {
        Result<string> matchedString = FromRegexMatch(EveryHourRegex(), input);
        return matchedString.IsSuccess
            ? FromEveryHourRecognition(matchedString.Value)
            : Result.Failure<PeriodicEveryHourRecognition>(
                "Unable to determine every hour recognition"
            );
    }

    private static Result<PeriodicEveryHourRecognition> FromEveryHourUnspecifiedRegex(string input)
    {
        Match match = EveryHourNotSpecifiedRegex().Match(input);
        return !match.Success
            ? Result.Failure<PeriodicEveryHourRecognition>(
                "Unable to determine every hour recognition"
            )
            : new PeriodicEveryHourRecognition(new SpecificTimeRecognition(1, 0));
    }

    private static Result<PeriodicEveryMinuteRecognition> FromEveryMinuteUnspecifiedRegex(
        string input
    )
    {
        Match match = EveryMinuteNotSpecifiedRegex().Match(input);
        return !match.Success
            ? Result.Failure<PeriodicEveryMinuteRecognition>(
                "Unable to determine Every Minute Recognition"
            )
            : new PeriodicEveryMinuteRecognition(new SpecificTimeRecognition(0, 1));
    }

    private static Result<string> FromRegexMatch(Regex regex, string input)
    {
        Match match = regex.Match(input);
        if (!match.Success || string.IsNullOrWhiteSpace(match.Groups[0].Value))
            return Result.Failure<string>("Unable to determine regex match.");
        return match.Groups[0].Value;
    }

    private static Recognitions.TimeRecognition FromMetadata(string metadata) =>
        metadata switch
        {
            "every_monday" => new PeriodicWeekDayRecognition(DayOfWeekRecognition.Monday),
            "every_tuesday" => new PeriodicWeekDayRecognition(DayOfWeekRecognition.Tuesday),
            "every_wednesday" => new PeriodicWeekDayRecognition(DayOfWeekRecognition.Wednesday),
            "every_thursday" => new PeriodicWeekDayRecognition(DayOfWeekRecognition.Thursday),
            "every_friday" => new PeriodicWeekDayRecognition(DayOfWeekRecognition.Friday),
            "every_saturday" => new PeriodicWeekDayRecognition(DayOfWeekRecognition.Saturday),
            "every_sunday" => new PeriodicWeekDayRecognition(DayOfWeekRecognition.Sunday),
            "every_day" => new PeriodicEveryDayRecognition(new RelativeRecognition(1)),
            _ => throw new UnreachableException(),
        };

    private static PeriodicEveryDayRecognition FromEveryDayRecognition(string text)
    {
        int offset = GetNumberOfOffset(text);
        return new PeriodicEveryDayRecognition(new RelativeRecognition(offset));
    }

    private static PeriodicEveryHourRecognition FromEveryHourRecognition(string text)
    {
        int offset = GetNumberOfOffset(text);
        return new PeriodicEveryHourRecognition(new SpecificTimeRecognition(offset, 0));
    }

    private static PeriodicEveryMinuteRecognition FromEveryMinuteRecognition(string text)
    {
        int offset = GetNumberOfOffset(text);
        return new PeriodicEveryMinuteRecognition(new SpecificTimeRecognition(0, offset));
    }

    private static int GetNumberOfOffset(string text)
    {
        ReadOnlySpan<char> onlyDigits = text.Where(char.IsDigit).ToArray();
        return onlyDigits.Length == 0 ? 1 : int.Parse(new string(onlyDigits));
    }

    private const RegexOptions Options =
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

    [GeneratedRegex(@"кажд(ый|ые|ую)\s+\d{1,2}\s*дн(я|ей)?", Options)]
    private static partial Regex EveryDayRegex();

    [GeneratedRegex(@"кажд(ый|ые|ую)\s+\d{1,2}\s*минут(у|ы)?", Options)]
    private static partial Regex EveryMinuteRegex();

    [GeneratedRegex(@"кажд(ый|ые|ую)\s+\d{1,2}\s*час(а|ов)?", Options)]
    private static partial Regex EveryHourRegex();

    [GeneratedRegex(@"(кажд(?:ый|ые|ую)\sча(?:с|а|ов))", Options)]
    private static partial Regex EveryHourNotSpecifiedRegex();

    [GeneratedRegex(@"кажд(?:ый|ые|ую)\sми(?:нуту|нут|н)", Options)]
    private static partial Regex EveryMinuteNotSpecifiedRegex();
}
