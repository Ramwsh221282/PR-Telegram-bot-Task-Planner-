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

public sealed class TestRemoveOwnerChatFacade : IClassFixture<DefaultTestsFixture>
{
    private readonly DefaultTestsFixture _fixture;

    public TestRemoveOwnerChatFacade(DefaultTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Invoke()
    {
        // тест первой регистрации пользователя
        const long userId = 123;
        const string userName = "Test User";
        const long chatId = 111;
        const string chatName = "Test Chat";
        const string zoneName = "Test Zone";

        var facade = _fixture.GetService<FirstUserChatRegistrationFacade>();
        Result registered = await facade.RegisterUser(userId, userName, chatId, chatName, zoneName);
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
        var notifications = _fixture.GetService<INotificationsReadableRepository>();
        Result<NotificationReceiver> receiver = await notifications.GetById(chatId);
        Assert.True(receiver.IsSuccess);

        Assert.Equal(chatId, receiver.Value.Id.Id);
        Assert.Equal(chatName, receiver.Value.Name.Name);
        Assert.Equal(zoneName, receiver.Value.TimeZone.ZoneName);

        // регистрация темы чата
        const long themeChatId = 15;
        var themeFacade = _fixture.GetService<UserThemeRegistrationFacade>();
        var addingTheme = await themeFacade.RegisterUserTheme(
            chatId,
            themeChatId,
            userId,
            chatName
        );
        Assert.True(addingTheme.IsSuccess);

        // проверка зарегистрированной темы
        registeredOwner = await externalChatsRepository.GetExternalChatOwnerById(userId);
        Assert.True(registeredOwner.IsSuccess);
        user = registeredOwner.Value;

        var theme = user.GetChildChat(
            ExternalChatId.Dedicated(chatId),
            ExternalChatId.Dedicated(themeChatId)
        );

        Assert.True(theme.IsSuccess);
        Assert.Equal(themeChatId, theme.Value.Id.Value);
        Assert.NotNull(theme.Value.ParentId);
        Assert.Equal(chatId, theme.Value.ParentId.Value.Value);

        // удаление чата пользователя (имеет всего 1 главный чат)
        var isUserLastChat = await externalChatsRepository.IsLastUserChat(userId);
        var removeOwnerChatFacade = _fixture.GetService<RemoveOwnerChatFacade>();
        Result removingOwnerChat = await removeOwnerChatFacade.RemoveOwnerChat(
            userId,
            chatId,
            isUserLastChat
        );
        Assert.True(removingOwnerChat.IsSuccess);

        receiver = await notifications.GetById(chatId);
        Assert.True(receiver.IsFailure);

        registeredOwner = await externalChatsRepository.GetExternalChatOwnerById(userId);
        Assert.True(registeredOwner.IsFailure);
    }
}
