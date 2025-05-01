using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;
using RocketTaskPlanner.Presenters.DependencyInjection;
using RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;
using RocketTaskPlanner.Utilities.UnixTimeUtilities;

namespace RocketTaskPlanner.Tests.RecognitionTests;

public sealed class TimeRecognitionTests
{
    private readonly IServiceProvider _provider;
    private readonly ITimeRecognitionFacade _facade = new TimeRecognitionFacade();
    private readonly TimeCalculationService _timeCalculation = new();
    private const long id = -1002678652435;

    public TimeRecognitionTests()
    {
        IServiceCollection services = new ServiceCollection();
        services.Inject();
        _provider = services.BuildServiceProvider();
    }

    private async Task<Result<ApplicationTimeZone>> GetApplicationTime(long id)
    {
        IApplicationTimeRepository<TimeZoneDbProvider> providerRepository =
            _provider.GetRequiredService<IApplicationTimeRepository<TimeZoneDbProvider>>();
        Result<TimeZoneDbProvider> provider = await providerRepository.Get();
        INotificationReceiverRepository receiversRepository =
            _provider.GetRequiredService<INotificationReceiverRepository>();
        Result<NotificationReceiver> receiver = await receiversRepository.GetById(id);
        await provider.Value.ProvideTimeZones();
        ApplicationTimeZone? timeZone = provider.Value.TimeZones.FirstOrDefault(tz =>
            tz.Name.Name == receiver.Value.TimeZone.ZoneName
        );
        return timeZone ?? Result.Failure<ApplicationTimeZone>("Time zone not found");
    }

    [Fact]
    public async Task Recognize_Time()
    {
        Result<ApplicationTimeZone> timeZone = await GetApplicationTime(id);
        Assert.True(timeZone.IsSuccess);

        string input1 = "Сегодня в 23 59 сделать коммит в гитхаб";
        string input2 = "Сегодня в 22:41 сделать коммит в гитхаб";
        string input3 = "Сегодня в 122 441 сделать коммит в гитхаб";
        string input4 = "Сегодня в 008:035 сделать коммит в гитхаб";

        Result<TimeRecognitionResult> recognition1 = await _facade.RecognizeTime(input1);
        Result<TimeRecognitionResult> recognition2 = await _facade.RecognizeTime(input2);
        Result<TimeRecognitionResult> recognition3 = await _facade.RecognizeTime(input3);
        Result<TimeRecognitionResult> recognition4 = await _facade.RecognizeTime(input4);

        Assert.True(recognition1.IsSuccess);
        Assert.True(recognition2.IsSuccess);
        Assert.True(recognition3.IsFailure);
        Assert.True(recognition4.IsFailure);
    }

    [Fact]
    public async Task Calculate_Time_With_Relative_Date()
    {
        Result<ApplicationTimeZone> timeZone = await GetApplicationTime(id);
        Assert.True(timeZone.IsSuccess);

        string input1 = "Сегодня в 00 30 сделать коммит в гитхаб";
        string input2 = "Сегодня в 22:41 сделать коммит в гитхаб";
        string input3 = "Завтра в 11:35 сделать коммит на гитхаб";
        string input4 = "Послезавтра в 12:45 сделать коммит на гитхаб";

        Result<TimeRecognitionResult> recognition1 = await _facade.RecognizeTime(input1);
        Result<TimeRecognitionResult> recognition2 = await _facade.RecognizeTime(input2);
        Result<TimeRecognitionResult> recognition3 = await _facade.RecognizeTime(input3);
        Result<TimeRecognitionResult> recognition4 = await _facade.RecognizeTime(input4);

        Assert.True(recognition1.IsSuccess);
        Assert.True(recognition2.IsSuccess);
        Assert.True(recognition3.IsSuccess);
        Assert.True(recognition4.IsSuccess);

        TimeZoneTimeInfo time = timeZone.Value.TimeInfo;

        TimeCalculationItem current = new(
            time.TimeStamp,
            time.DateTime,
            recognition1.Value.IsPeriodic
        );

        int expectedInput1Day = time.DateTime.Day;
        int expectedInput1Hour = 23;
        int expectedInput1Minute = 59;

        int expectedInput2Day = time.DateTime.Day;
        int expectedInput2Hour = 22;
        int expectedInput2Minute = 41;

        int expectedInput3Day = time.DateTime.Day + 1;
        int expectedInput3Hour = 11;
        int expectedInput3Minute = 35;

        int expectedInput4Day = time.DateTime.Day + 2;
        int expectedInput4Hour = 12;
        int expectedInput4Minute = 45;

        TimeCalculationItem calculated1 = _timeCalculation.AddOffset(current, recognition1.Value);

        TimeCalculationItem calculated2 = _timeCalculation.AddOffset(current, recognition2.Value);
        TimeCalculationItem calculated3 = _timeCalculation.AddOffset(current, recognition3.Value);
        TimeCalculationItem calculated4 = _timeCalculation.AddOffset(current, recognition4.Value);

        Assert.Equal(expectedInput1Day, calculated1.CalculationDateTime.Day);
        Assert.Equal(expectedInput1Hour, calculated1.CalculationDateTime.Hour);
        Assert.Equal(expectedInput1Minute, calculated1.CalculationDateTime.Minute);

        Assert.Equal(expectedInput2Day, calculated2.CalculationDateTime.Day);
        Assert.Equal(expectedInput2Hour, calculated2.CalculationDateTime.Hour);
        Assert.Equal(expectedInput2Minute, calculated2.CalculationDateTime.Minute);

        Assert.Equal(expectedInput3Day, calculated3.CalculationDateTime.Day);
        Assert.Equal(expectedInput3Hour, calculated3.CalculationDateTime.Hour);
        Assert.Equal(expectedInput3Minute, calculated3.CalculationDateTime.Minute);

        Assert.Equal(expectedInput4Day, calculated4.CalculationDateTime.Day);
        Assert.Equal(expectedInput4Hour, calculated4.CalculationDateTime.Hour);
        Assert.Equal(expectedInput4Minute, calculated4.CalculationDateTime.Minute);
    }

    [Fact]
    public async Task Calculate_Time_With_Month_Date()
    {
        Result<ApplicationTimeZone> timeZone = await GetApplicationTime(id);
        Assert.True(timeZone.IsSuccess);

        TimeZoneTimeInfo time = timeZone.Value.TimeInfo;

        (string input, int month, int day, int hour, int minute)[] testData =
        [
            ("14 января в 13:55", 1, 14, 13, 55),
            ("8 февраля в 9:55", 2, 8, 9, 55),
            ("20 марта в 18:00", 3, 20, 18, 0),
            ("10 апреля в 16:45", 4, 10, 16, 45),
            ("5 мая в 7:30", 5, 5, 7, 30),
            ("22 июня в 12:15", 6, 22, 12, 15),
            ("1 июля в 23:59", 7, 1, 23, 59),
            ("18 августа в 11:40", 8, 18, 11, 40),
            ("25 сентября в 6:20", 9, 25, 6, 20),
            ("3 октября в 14:50", 10, 3, 14, 50),
            ("12 ноября в 20:10", 11, 12, 20, 10),
            ("31 декабря в 4:35", 12, 31, 4, 35),
        ];

        foreach ((string input, int month, int day, int hour, int minute) testInput in testData)
        {
            Result<TimeRecognitionResult> recognition = await _facade.RecognizeTime(
                testInput.input
            );
            Assert.True(recognition.IsSuccess);
            TimeCalculationItem item = new TimeCalculationItem(
                time.TimeStamp,
                time.DateTime,
                recognition.Value.IsPeriodic
            );
            TimeCalculationItem calculated = _timeCalculation.AddOffset(item, recognition.Value);

            int calculatedDay = calculated.CalculationDateTime.Day;
            int calculatedHour = calculated.CalculationDateTime.Hour;
            int calculatedMinute = calculated.CalculationDateTime.Minute;
            int calculatedMonth = calculated.CalculationDateTime.Month;

            Assert.Equal(testInput.day, calculatedDay);
            Assert.Equal(testInput.hour, calculatedHour);
            Assert.Equal(testInput.minute, calculatedMinute);
            Assert.Equal(testInput.month, calculatedMonth);
        }
    }

    [Fact]
    public async Task Calculate_Periodic_Time()
    {
        Result<ApplicationTimeZone> timeZone = await GetApplicationTime(id);
        Assert.True(timeZone.IsSuccess);

        TimeZoneTimeInfo time = timeZone.Value.TimeInfo;
        string input1 = "Каждый день в 13:45 я занимаюсь спортом";
        string input2 = "Каждые выходные мы едем на дачу";
        string input3 = "С каждой недельной встречей все становится лучше";
        string input4 = "Каждый понедельник в 14:33 я встречаюсь с друзьями";
        string input5 = "Каждую среду в 14:33 я встречаюсь с друзьями";
        string input6 = "Каждую пятницу в 14:33 я встречаюсь с друзьями";

        Result<TimeRecognitionResult> recognition1 = await _facade.RecognizeTime(input1);
        Result<TimeRecognitionResult> recognition2 = await _facade.RecognizeTime(input2);
        Result<TimeRecognitionResult> recognition3 = await _facade.RecognizeTime(input3);
        Result<TimeRecognitionResult> recognition4 = await _facade.RecognizeTime(input4);
        Result<TimeRecognitionResult> recognition5 = await _facade.RecognizeTime(input5);
        Result<TimeRecognitionResult> recognition6 = await _facade.RecognizeTime(input6);

        Assert.True(recognition1.IsSuccess);
        Assert.True(recognition2.IsFailure);
        Assert.True(recognition3.IsFailure);
        Assert.True(recognition4.IsSuccess);
        Assert.True(recognition5.IsSuccess);
        Assert.True(recognition6.IsSuccess);

        TimeCalculationItem current1 = new(
            time.TimeStamp,
            time.DateTime,
            recognition1.Value.IsPeriodic
        );
        TimeCalculationItem current4 = new(
            time.TimeStamp,
            time.DateTime,
            recognition4.Value.IsPeriodic
        );
        TimeCalculationItem current5 = new(
            time.TimeStamp,
            time.DateTime,
            recognition5.Value.IsPeriodic
        );
        TimeCalculationItem current6 = new(
            time.TimeStamp,
            time.DateTime,
            recognition6.Value.IsPeriodic
        );

        TimeCalculationItem calculated1 = _timeCalculation.AddOffset(current1, recognition1.Value);

        Assert.Equal(current1.CalculationDateTime.Day + 1, calculated1.CalculationDateTime.Day);

        TimeCalculationItem calculated4 = _timeCalculation.AddOffset(current4, recognition4.Value);

        Assert.Equal(DayOfWeek.Monday, calculated4.CalculationDateTime.DayOfWeek);

        TimeCalculationItem calculated5 = _timeCalculation.AddOffset(current5, recognition5.Value);

        Assert.Equal(DayOfWeek.Wednesday, calculated5.CalculationDateTime.DayOfWeek);

        TimeCalculationItem calculated6 = _timeCalculation.AddOffset(current6, recognition6.Value);

        Assert.Equal(DayOfWeek.Friday, calculated6.CalculationDateTime.DayOfWeek);
    }
}
