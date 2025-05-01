namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;

public abstract record TimeRecognitionTicket;

public sealed record PeriodicTimeRecognitionTicket(string Input) : TimeRecognitionTicket;

public sealed record SingleTimeRecognitionTicket(string Input) : TimeRecognitionTicket;

public sealed record UnknownTimeRecognitionTicket : TimeRecognitionTicket;

public static class TimeRecognitionTicketModifiers
{
    public static TimeRecognitionResult ApplyModification(
        this TimeRecognitionTicket ticket,
        TimeRecognitionResult result
    ) =>
        ticket switch
        {
            PeriodicTimeRecognitionTicket => result.ApplyPeriodicTimeSchedule(),
            _ => result,
        };

    private static TimeRecognitionResult ApplyPeriodicTimeSchedule(
        this TimeRecognitionResult result
    ) => result with { IsPeriodic = true };
}
