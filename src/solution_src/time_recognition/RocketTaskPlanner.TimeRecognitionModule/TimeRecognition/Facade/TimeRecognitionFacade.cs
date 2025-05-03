using CSharpFunctionalExtensions;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade.Services;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade;

public sealed class TimeRecognitionFacade : ITimeRecognitionFacade
{
    private readonly TimeRecognitionTicketService _tickets;
    private readonly TimeRecognitionMetadataService _metadata;

    public TimeRecognitionFacade()
    {
        TimeRecognitionRecognizersService recognizers = new();
        _tickets = new TimeRecognitionTicketService(recognizers);
        _metadata = new TimeRecognitionMetadataService(recognizers);
    }

    public async Task<Result<TimeRecognitionResult>> RecognizeTime(string? input)
    {
        // создание Dto модели для распознавания времени
        Result<TimeRecognitionTicket> ticket = await CreateTicket(input);
        if (ticket.IsFailure)
            return Result.Failure<TimeRecognitionResult>(ticket.Error);

        // сбор метаданных (непосредственно распознавание)
        Result<RecognitionMetadataCollection> metadata = await CollectMetadata(ticket.Value);
        if (metadata.IsFailure)
            return Result.Failure<TimeRecognitionResult>(metadata.Error);

        return ticket.Value switch
        {
            // если время не периодичное
            SingleTimeRecognitionTicket => new TimeRecognitionResult(metadata.Value, false),

            // если время периодичное
            PeriodicTimeRecognitionTicket => new TimeRecognitionResult(metadata.Value, true),

            // если распознаваний нет
            _ => Result.Failure<TimeRecognitionResult>("Unknown time recognition ticket"),
        };
    }

    public async Task<Result<TimeRecognitionTicket>> CreateTicket(string? input)
    {
        TimeRecognitionTicket ticket = await _tickets.CreateRecognitionTicket(input);
        return await _tickets.CreateRecognitionTicket(input) switch
        {
            UnknownTimeRecognitionTicket => Result.Failure<TimeRecognitionTicket>(
                "Не удалось распознать время задачи."
            ),
            _ => ticket,
        };
    }

    private async Task<Result<RecognitionMetadataCollection>> CollectMetadata(
        TimeRecognitionTicket ticket
    )
    {
        RecognitionMetadataCollection metadata = await _metadata.CollectMetadata(ticket);
        return metadata.Count == 0
            ? Result.Failure<RecognitionMetadataCollection>("Не удалось распознать время задачи.")
            : metadata;
    }
}
