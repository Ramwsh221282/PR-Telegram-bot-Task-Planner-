using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.Facades;
using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.Entities;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Tests.TestDependencies;

namespace RocketTaskPlanner.Tests.FacadesTests;

public sealed class TestUserFirstRegistration : IClassFixture<DefaultTestsFixture>
{
    private readonly DefaultTestsFixture _fixture;

    public TestUserFirstRegistration(DefaultTestsFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Invoke()
    {
        // тест первой регистрации пользователя
        const long userId = 123;
        const string userName = "Test User";
        const long chatId = 111;
        const string chatName = "Test Chat";
        const string zoneName = "Test Zone";

        var firstRegistrationFacade = _fixture.GetService<FirstUserChatRegistrationFacade>();
        Result registered = await firstRegistrationFacade.RegisterUser(
            userId,
            userName,
            chatId,
            chatName,
            zoneName
        );
        Assert.True(registered.IsSuccess);

        // проверка на существование зарегистрированного пользователя, и его основного чата.
        var externalChatsRepository = _fixture.GetService<IExternalChatsReadableRepository>();
        var registeredOwner = await externalChatsRepository.GetExternalChatOwnerById(userId);

        Assert.True(registeredOwner.IsSuccess);
        ExternalChatOwner user = registeredOwner.Value;
        Assert.Equal(userId, user.Id.Value);
        Assert.Equal(userName, user.Name.Value);

        Result<ExternalChat> parentChat = user.GetParentChat(ExternalChatId.Dedicated(chatId));
        Assert.True(parentChat.IsSuccess);
        Assert.Equal(chatId, parentChat.Value.Id.Value);
        Assert.Equal(chatName, parentChat.Value.Name.Value);
        Assert.Null(parentChat.Value.ParentId);

        // проверка на существование зарегистрированного чата для уведомлений
        var notifications = _fixture.GetService<INotificationRepository>();
        Result<NotificationReceiver> receiver = await notifications.Readable.GetById(chatId);
        Assert.True(receiver.IsSuccess);

        Assert.Equal(chatId, receiver.Value.Id.Id);
        Assert.Equal(chatName, receiver.Value.Name.Name);
        Assert.Equal(zoneName, receiver.Value.TimeZone.ZoneName);

        // тест добавления другого чата пользователю
        const long otherChatId = 456;
        const string otherChatName = "Test Other Name";
        const string otherZoneName = "Test Other Zone";

        // добавление другого чата пользователю
        var addNewChatFacade = _fixture.GetService<UserChatRegistrationFacade>();
        var addingOtherChat = await addNewChatFacade.AddUserExternalChat(
            userId,
            otherChatId,
            otherChatName,
            otherZoneName
        );
        Assert.True(addingOtherChat.IsSuccess);

        // проверка на существование зарегистрированного другого чата
        registeredOwner = await externalChatsRepository.GetExternalChatOwnerById(userId);
        Assert.True(registeredOwner.IsSuccess);
        user = registeredOwner.Value;
        Assert.Equal(userId, user.Id.Value);
        Assert.Equal(userName, user.Name.Value);

        parentChat = user.GetParentChat(ExternalChatId.Dedicated(otherChatId));
        Assert.True(parentChat.IsSuccess);
        Assert.Equal(otherChatId, parentChat.Value.Id.Value);
        Assert.Equal(otherChatName, parentChat.Value.Name.Value);
        Assert.Null(parentChat.Value.ParentId);

        // проверка на существование зарегистрированного чата для уведомлений
        receiver = await notifications.Readable.GetById(otherChatId);
        Assert.True(receiver.IsSuccess);

        Assert.Equal(otherChatId, receiver.Value.Id.Id);
        Assert.Equal(otherChatName, receiver.Value.Name.Name);
        Assert.Equal(otherZoneName, receiver.Value.TimeZone.ZoneName);
    }
}
