using CSharpFunctionalExtensions;
using RocketTaskPlanner.Tests.TestDependencies;
using RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;
using Serilog;

namespace RocketTaskPlanner.Tests.RecognitionTests;

public sealed class TicketRecognitionTests : IClassFixture<DefaultTestsFixture>
{
    private readonly DefaultTestsFixture _fixture;

    public TicketRecognitionTests(DefaultTestsFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Recognize_Non_Periodic_Time_Success()
    {
        TimeRecognitionFacade facade = _fixture.GetService<TimeRecognitionFacade>();
        TimeCalculationService calculation = _fixture.GetService<TimeCalculationService>();
        
        ILogger logger = _fixture.GetService<ILogger>();
        
        string input = "В понедельник в 16:35 будет собрание.";
        
        logger.Information("Input text: {text}", input);
        
        Result<TimeRecognitionTicket> ticket = await facade.CreateTicket(input);
        
        Action<ILogger> loggerAction = ticket.IsSuccess
            ? log => log.Information("Ticket recognized: {true}", ticket.IsSuccess)
            : log => log.Error("Ticket recognized: {false}", ticket.IsSuccess);

        loggerAction(logger);
        
        Assert.True(ticket.IsSuccess);
        Assert.IsType<SingleTimeRecognitionTicket>(ticket.Value);

        var recognitionResult = await facade.RecognizeTime(input);
        TimeCalculationItem current = new TimeCalculationItem(DateTime.Now, false);
        
        var calculated = calculation.AddOffset(current, recognitionResult.Value);
        
        logger.Information("Recognized date time offset: {dateTime}", calculated.CalculationDateTime);
    }

    [Fact]
    public async Task Recognize_Periodic_Time_Success()
    {
        TimeRecognitionFacade facade = _fixture.GetService<TimeRecognitionFacade>();
        string input = "Каждый понедельник в 20:55 будет происходить собрание.";
        Result<TimeRecognitionTicket> ticket = await facade.CreateTicket(input);
        Assert.True(ticket.IsSuccess);
        Assert.IsType<PeriodicTimeRecognitionTicket>(ticket.Value);
    }

    [Fact]
    public async Task Collect_Recognition_Metadata_From_Date_WithoutTime_Failure()
    {
        TimeRecognitionFacade facade = _fixture.GetService<TimeRecognitionFacade>();
        string input = "В понедельник будет происходить собрание";
        Result<TimeRecognitionTicket> ticket = await facade.CreateTicket(input);
        Assert.True(ticket.IsFailure);
    }

    [Fact]
    public async Task Create_Recognition_Ticket_With_Month_Day_Time_Success()
    {
        TimeRecognitionFacade facade = _fixture.GetService<TimeRecognitionFacade>();
        TimeCalculationService calculation = _fixture.GetService<TimeCalculationService>();
        ILogger logger = _fixture.GetService<ILogger>();
        
        string input = "20 мая в 9:35 сделать зарядку.";
        logger.Information("Input text: {text}", input);
        
        Result<TimeRecognitionTicket> ticket = await facade.CreateTicket(input);
        Assert.True(ticket.IsSuccess);
        Assert.IsType<SingleTimeRecognitionTicket>(ticket.Value);
        
        Action<ILogger> loggerAction = ticket.IsSuccess
            ? log => log.Information("Ticket recognized: {true}", ticket.IsSuccess)
            : log => log.Error("Ticket recognized: {false}", ticket.IsSuccess);
        
        loggerAction(logger);
        
        var recognitionResult = await facade.RecognizeTime(input);
        TimeCalculationItem current = new TimeCalculationItem(DateTime.Now, false);
        
        var calculated = calculation.AddOffset(current, recognitionResult.Value);
        
        logger.Information("Recognized date time offset: {dateTime}", calculated.CalculationDateTime);
    }

    [Fact]
    public async Task Collect_Recognition_Metadata_From_NoTimeContext_Failure()
    {
        TimeRecognitionFacade facade = _fixture.GetService<TimeRecognitionFacade>();
        string input = "Это сообщение не имеет контекста времени.";
        ILogger logger = _fixture.GetService<ILogger>();
        
        logger.Information("Input text: {text}", input);
        
        Result<TimeRecognitionTicket> ticket = await facade.CreateTicket(input);
        Assert.True(ticket.IsFailure);
        
        Action<ILogger> loggerAction = ticket.IsSuccess
            ? log => log.Information("Ticket recognized: {true}", ticket.IsSuccess)
            : log => log.Error("Ticket recognized: {false}", ticket.IsSuccess);
        
        loggerAction(logger);
    }
}
