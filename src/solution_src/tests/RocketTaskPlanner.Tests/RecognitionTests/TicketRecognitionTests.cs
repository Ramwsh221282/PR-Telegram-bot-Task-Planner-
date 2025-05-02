using CSharpFunctionalExtensions;
using RocketTaskPlanner.Tests.TestDependencies;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;

namespace RocketTaskPlanner.Tests.RecognitionTests;

public sealed class TicketRecognitionTests : IClassFixture<DefaultTestsFixture>
{
    private readonly DefaultTestsFixture _fixture;

    public TicketRecognitionTests(DefaultTestsFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Recognize_Non_Periodic_Time_Success()
    {
        TimeRecognitionFacade facade = _fixture.GetService<TimeRecognitionFacade>();
        string input = "В понедельник в 14:35 будет собрание.";
        Result<TimeRecognitionTicket> ticket = await facade.CreateTicket(input);
        Assert.True(ticket.IsSuccess);
        Assert.IsType<SingleTimeRecognitionTicket>(ticket.Value);
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
        string input = "1 Апреля в 9:35 сделать зарядку.";
        Result<TimeRecognitionTicket> ticket = await facade.CreateTicket(input);
        Assert.True(ticket.IsSuccess);
        Assert.IsType<SingleTimeRecognitionTicket>(ticket.Value);
    }

    [Fact]
    public async Task Collect_Recognition_Metadata_From_NoTimeContext_Failure()
    {
        TimeRecognitionFacade facade = _fixture.GetService<TimeRecognitionFacade>();
        string input = "Это сообщение не имеет контекста времени.";
        Result<TimeRecognitionTicket> ticket = await facade.CreateTicket(input);
        Assert.True(ticket.IsFailure);
    }
}
