using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;
using RocketTaskPlanner.Presenters.DependencyInjection;

namespace RocketTaskPlanner.Tests.SqliteTests;

public sealed class NotificationReceiverAggregateTests
{
    private readonly IServiceScopeFactory _factory;

    public NotificationReceiverAggregateTests()
    {
        IServiceCollection services = new ServiceCollection();
        services.Inject();
        IServiceProvider provider = services.BuildServiceProvider();
        _factory = provider.GetRequiredService<IServiceScopeFactory>();
    }

    [Fact]
    public async Task Test_Create_Delete_Notification_Receiver()
    {
        IServiceScope scope = _factory.CreateScope();
        IServiceProvider provider = scope.ServiceProvider;
        INotificationReceiverRepository repository =
            provider.GetRequiredService<INotificationReceiverRepository>();

        NotificationReceiverId id = NotificationReceiverId.Create(123).Value;
        NotificationReceiverName name = NotificationReceiverName.Create("Test Receiver").Value;
        NotificationReceiverTimeZone timeZone = NotificationReceiverTimeZone
            .Create("Test Zone Name", 123)
            .Value;

        NotificationReceiver receiver = new NotificationReceiver()
        {
            Id = id,
            Name = name,
            TimeZone = timeZone,
        };

        Result addResult = await repository.Add(receiver);
        Assert.True(addResult.IsSuccess);

        Result deleteResult = await repository.Remove(receiver.Id.Id);
        Assert.True(deleteResult.IsSuccess);
    }

    [Fact]
    public async Task Test_Create_Delete_GetById_Notification_Receiver()
    {
        IServiceScope scope = _factory.CreateScope();
        IServiceProvider provider = scope.ServiceProvider;
        INotificationReceiverRepository repository =
            provider.GetRequiredService<INotificationReceiverRepository>();

        NotificationReceiverId id = NotificationReceiverId.Create(123).Value;
        NotificationReceiverName name = NotificationReceiverName.Create("Test Receiver").Value;
        NotificationReceiverTimeZone timeZone = NotificationReceiverTimeZone
            .Create("Test Zone Name", 123)
            .Value;

        NotificationReceiver receiver = new()
        {
            Id = id,
            Name = name,
            TimeZone = timeZone,
        };

        Result addResult = await repository.Add(receiver);
        Assert.True(addResult.IsSuccess);

        bool noExceptions = true;
        try
        {
            Result<NotificationReceiver> receiverFromDb = await repository.GetById(receiver.Id.Id);
            Assert.True(receiverFromDb.IsSuccess);
        }
        catch
        {
            noExceptions = false;
        }

        Result deleteResult = await repository.Remove(receiver.Id.Id);
        Assert.True(deleteResult.IsSuccess);
        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Test_Create_Delete_Receiver_With_General_Chat_Notification_Subject()
    {
        IServiceScope scope = _factory.CreateScope();
        IServiceProvider provider = scope.ServiceProvider;
        INotificationReceiverRepository repository =
            provider.GetRequiredService<INotificationReceiverRepository>();

        NotificationReceiverId id = NotificationReceiverId.Create(123).Value;
        NotificationReceiverName name = NotificationReceiverName.Create("Test Receiver").Value;
        NotificationReceiverTimeZone timeZone = NotificationReceiverTimeZone
            .Create("Test Zone Name", 123)
            .Value;

        NotificationReceiver receiver = new NotificationReceiver()
        {
            Id = id,
            Name = name,
            TimeZone = timeZone,
        };

        Result addResult = await repository.Add(receiver);
        Assert.True(addResult.IsSuccess);

        ReceiverSubjectId subjectId = ReceiverSubjectId.Create(66321).Value;
        ReceiverSubjectDateCreated created = new(DateTime.Now);
        ReceiverSubjectDateNotify dateNotify = new(DateTime.Now);
        ReceiverSubjectTimeInfo time = new(created, dateNotify);
        ReceiverSubjectMessage message = ReceiverSubjectMessage.Create("My message").Value;
        ReceiverSubjectPeriodInfo period = new(false);

        GeneralChatReceiverSubject subject = receiver.AddSubject(subjectId, time, period, message);

        await repository.AddSubject(subject);

        Result<NotificationReceiver> fromDb = await repository.GetById(receiver.Id.Id);

        Assert.True(fromDb.IsSuccess);

        bool containsSubject = fromDb.Value.Subjects.Any(s => s.Id == subject.Id);
        Assert.True(containsSubject);

        Result deleteResult = await repository.Remove(receiver.Id.Id);
        Assert.True(deleteResult.IsSuccess);
    }

    [Fact]
    public async Task Test_Create_Delete_Receiver_With_General_Chat_Notification_Subject_And_Theme()
    {
        IServiceScope scope = _factory.CreateScope();
        IServiceProvider provider = scope.ServiceProvider;
        INotificationReceiverRepository repository =
            provider.GetRequiredService<INotificationReceiverRepository>();

        NotificationReceiverId id = NotificationReceiverId.Create(123).Value;
        NotificationReceiverName name = NotificationReceiverName.Create("Test Receiver").Value;
        NotificationReceiverTimeZone timeZone = NotificationReceiverTimeZone
            .Create("Test Zone Name", 123)
            .Value;

        NotificationReceiver receiver = new NotificationReceiver()
        {
            Id = id,
            Name = name,
            TimeZone = timeZone,
        };

        Result addResult = await repository.Add(receiver);
        Assert.True(addResult.IsSuccess);

        ReceiverSubjectId subjectId = ReceiverSubjectId.Create(66321).Value;
        ReceiverSubjectDateCreated created = new(DateTime.Now);
        ReceiverSubjectDateNotify dateNotify = new(DateTime.Now);
        ReceiverSubjectTimeInfo time = new(created, dateNotify);
        ReceiverSubjectMessage message = ReceiverSubjectMessage.Create("My message").Value;
        ReceiverSubjectPeriodInfo period = new(false);

        GeneralChatReceiverSubject subject = receiver.AddSubject(subjectId, time, period, message);

        await repository.AddSubject(subject);

        ReceiverThemeId themeId = ReceiverThemeId.Create(355521).Value;
        Result<ReceiverTheme> theme = receiver.AddTheme(themeId);
        Assert.True(theme.IsSuccess);

        await repository.AddTheme(theme.Value);

        Result<NotificationReceiver> fromDb = await repository.GetById(receiver.Id.Id);

        Assert.True(fromDb.IsSuccess);

        bool containsSubject = fromDb.Value.Subjects.Any(s => s.Id == subject.Id);
        Assert.True(containsSubject);

        bool containsTheme = fromDb.Value.Themes.Any(t => t.Id == themeId);
        Assert.True(containsTheme);

        Result deleteResult = await repository.Remove(receiver.Id.Id);
        Assert.True(deleteResult.IsSuccess);
    }

    [Fact]
    public async Task Test_Create_Delete_Receiver_With_General_Chat_Subject_And_Theme_And_Theme_Subject()
    {
        IServiceScope scope = _factory.CreateScope();
        IServiceProvider provider = scope.ServiceProvider;
        INotificationReceiverRepository repository =
            provider.GetRequiredService<INotificationReceiverRepository>();

        NotificationReceiverId id = NotificationReceiverId.Create(123).Value;
        NotificationReceiverName name = NotificationReceiverName.Create("Test Receiver").Value;
        NotificationReceiverTimeZone timeZone = NotificationReceiverTimeZone
            .Create("Test Zone Name", 123)
            .Value;

        NotificationReceiver receiver = new NotificationReceiver()
        {
            Id = id,
            Name = name,
            TimeZone = timeZone,
        };

        Result addResult = await repository.Add(receiver);
        Assert.True(addResult.IsSuccess);

        ReceiverSubjectId subjectId = ReceiverSubjectId.Create(66321).Value;
        ReceiverSubjectDateCreated created = new(DateTime.Now);
        ReceiverSubjectDateNotify dateNotify = new(DateTime.Now);
        ReceiverSubjectTimeInfo time = new(created, dateNotify);
        ReceiverSubjectMessage message = ReceiverSubjectMessage.Create("My message").Value;
        ReceiverSubjectPeriodInfo period = new(true);

        GeneralChatReceiverSubject subject = receiver.AddSubject(subjectId, time, period, message);

        await repository.AddSubject(subject);

        ReceiverThemeId themeId = ReceiverThemeId.Create(355521).Value;
        Result<ReceiverTheme> theme = receiver.AddTheme(themeId);
        Assert.True(theme.IsSuccess);

        await repository.AddTheme(theme.Value);

        ReceiverTheme addedTheme = receiver.Themes.FirstOrDefault(t => t.Id == themeId)!;

        ThemeChatSubject themeChatSubject = addedTheme.AddSubject(subjectId, time, period, message);

        await repository.AddSubject(themeChatSubject);

        Result<NotificationReceiver> fromDb = await repository.GetById(receiver.Id.Id);

        Assert.True(fromDb.IsSuccess);

        bool containsSubject = fromDb.Value.Subjects.Any(s => s.Id == subject.Id);
        Assert.True(containsSubject);

        bool containsTheme = fromDb.Value.Themes.Any(t => t.Id == themeId);
        Assert.True(containsTheme);

        ReceiverTheme themeWithSubject = fromDb.Value.Themes.FirstOrDefault(t => t.Id == themeId)!;

        bool themeContainsSubject = themeWithSubject.Subjects.Any(s => s.Id == subject.Id);
        Assert.True(themeContainsSubject);

        Result deleteResult = await repository.Remove(receiver.Id.Id);
        Assert.True(deleteResult.IsSuccess);
    }
}
