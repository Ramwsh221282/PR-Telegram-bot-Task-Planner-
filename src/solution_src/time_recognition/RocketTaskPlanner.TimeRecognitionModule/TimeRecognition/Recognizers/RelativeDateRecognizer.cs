﻿using System.Diagnostics;
using Build5Nines.SharpVector;
using CSharpFunctionalExtensions;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognizers;

/// <summary>
/// Класс для распознавания относительных дат
/// </summary>
public sealed class RelativeDateRecognizer : BasicVectorRecognizer
{
    public RelativeDateRecognizer()
        : base(0.6f)
    {
        AddText("сегодня", "today");
        AddText("завтра", "yesterday");
        AddText("послезавтра", "dayaftertomorrow");
        AddText("после завтра", "dayaftertomorrow");
    }

    public override async Task<Recognitions.TimeRecognition> TryRecognize(string input)
    {
        IVectorTextResult<string, string> result = await PerformRecognition(input);
        Result<IVectorTextResultItem<string, string>> mostAccurate = GetMostAccurateRecognition(
            result
        );
        return mostAccurate.IsSuccess switch
        {
            true => MatchFromMetadata(mostAccurate.Value.Metadata!),
            false => new UnrecognizedTime(),
        };
    }

    private static RelativeRecognition MatchFromMetadata(string metaData) =>
        metaData switch
        {
            "today" => new RelativeRecognition(0),
            "yesterday" => new RelativeRecognition(1),
            "dayaftertomorrow" => new RelativeRecognition(2),
            _ => throw new UnreachableException(),
        };
}
