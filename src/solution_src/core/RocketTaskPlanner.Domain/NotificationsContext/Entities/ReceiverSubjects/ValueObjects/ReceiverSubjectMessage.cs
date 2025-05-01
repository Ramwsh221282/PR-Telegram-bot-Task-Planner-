namespace RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;

public sealed record ReceiverSubjectMessage
{
    public string Message { get; }

    private ReceiverSubjectMessage() => Message = string.Empty; // ef core

    private ReceiverSubjectMessage(string message) => Message = message;

    public static Result<ReceiverSubjectMessage> Create(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return Result.Failure<ReceiverSubjectMessage>("Текст сообщения был пустым.");

        string formatted = message.Trim();

        if (string.IsNullOrWhiteSpace(formatted))
            return Result.Failure<ReceiverSubjectMessage>("Текст сообщения был пустым.");

        return new ReceiverSubjectMessage(formatted);
    }
}
