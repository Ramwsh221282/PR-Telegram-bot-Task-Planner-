using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChat;
using RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChatTheme;
using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterChat;
using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterTheme;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Tests.TestDependencies;

namespace RocketTaskPlanner.Tests.NotificationContext;

public sealed class NotificationsUseCasesTests : IClassFixture<DefaultTestsFixture>
{
    private readonly DefaultTestsFixture _fixture;

    public NotificationsUseCasesTests(DefaultTestsFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Test_Register_General_Chat_Success()
    {
        const long receiverId = 123;
        const string receiverName = "Test Receiver";
        const string receiverZoneName = "Test Time Zone Name";

        RegisterChatUseCase useCase = new(receiverId, receiverName, receiverZoneName);

        var useCaseHandler = _fixture.GetService<
            IUseCaseHandler<RegisterChatUseCase, RegisterChatUseCaseResponse>
        >();

        Result<RegisterChatUseCaseResponse> result = await useCaseHandler.Handle(useCase);
        Assert.True(result.IsSuccess);

        RegisterChatUseCaseResponse response = result.Value;
        Assert.Equal(receiverId, response.RegisteredChatId);
        Assert.Equal(receiverName, response.RegisteredChatName);
    }

    [Fact]
    public async Task Test_Register_General_Duplicate_Chat_Failure()
    {
        const long receiverId = 123;
        const string receiverName = "Test Receiver";
        const string receiverZoneName = "Test Time Zone Name";

        RegisterChatUseCase useCase = new(receiverId, receiverName, receiverZoneName);

        var useCaseHandler = _fixture.GetService<
            IUseCaseHandler<RegisterChatUseCase, RegisterChatUseCaseResponse>
        >();

        Result<RegisterChatUseCaseResponse> result = await useCaseHandler.Handle(useCase);
        Assert.True(result.IsSuccess);

        RegisterChatUseCaseResponse response = result.Value;
        Assert.Equal(receiverId, response.RegisteredChatId);
        Assert.Equal(receiverName, response.RegisteredChatName);

        Result<RegisterChatUseCaseResponse> duplicate = await useCaseHandler.Handle(useCase);
        Assert.False(duplicate.IsSuccess);
    }

    [Fact]
    public async Task Test_Create_General_Chat_Subject_Success()
    {
        const long receiverId = 123;
        const string receiverName = "Test Receiver";
        const string receiverZoneName = "Test Time Zone Name";

        RegisterChatUseCase useCase = new(receiverId, receiverName, receiverZoneName);

        var useCaseHandler = _fixture.GetService<
            IUseCaseHandler<RegisterChatUseCase, RegisterChatUseCaseResponse>
        >();

        Result<RegisterChatUseCaseResponse> result = await useCaseHandler.Handle(useCase);
        Assert.True(result.IsSuccess);

        RegisterChatUseCaseResponse response = result.Value;
        Assert.Equal(receiverId, response.RegisteredChatId);
        Assert.Equal(receiverName, response.RegisteredChatName);

        const long subjectId = 513;
        const string subjectMessage = "Test Subject Message";
        DateTime dateCreated = DateTime.UtcNow;
        DateTime dateNotify = dateCreated.AddMinutes(1);

        CreateTaskForChatUseCase taskUseCase = new(
            receiverId,
            subjectId,
            dateCreated,
            dateNotify,
            subjectMessage,
            false
        );
        var taskHandler = _fixture.GetService<
            IUseCaseHandler<CreateTaskForChatUseCase, CreateTaskForChatUseCaseResponse>
        >();

        Result<CreateTaskForChatUseCaseResponse> responseResult = await taskHandler.Handle(
            taskUseCase
        );
        Assert.True(responseResult.IsSuccess);
    }

    [Fact]
    public async Task Test_Create_Theme_Chat_For_General_Chat_Success()
    {
        const long receiverId = 123;
        const string receiverName = "Test Receiver";
        const string receiverZoneName = "Test Time Zone Name";

        RegisterChatUseCase useCase = new(receiverId, receiverName, receiverZoneName);

        var useCaseHandler = _fixture.GetService<
            IUseCaseHandler<RegisterChatUseCase, RegisterChatUseCaseResponse>
        >();

        Result<RegisterChatUseCaseResponse> result = await useCaseHandler.Handle(useCase);
        Assert.True(result.IsSuccess);

        RegisterChatUseCaseResponse response = result.Value;
        Assert.Equal(receiverId, response.RegisteredChatId);
        Assert.Equal(receiverName, response.RegisteredChatName);

        long themeId = 5;
        RegisterThemeUseCase themeUseCase = new(receiverId, themeId);
        var themeHandler = _fixture.GetService<
            IUseCaseHandler<RegisterThemeUseCase, RegisterThemeResponse>
        >();
        Result<RegisterThemeResponse> themeResult = await themeHandler.Handle(themeUseCase);
        Assert.True(themeResult.IsSuccess);
    }

    [Fact]
    public async Task Create_Subject_For_Theme_Chat_Success()
    {
        const long receiverId = 123;
        const string receiverName = "Test Receiver";
        const string receiverZoneName = "Test Time Zone Name";

        RegisterChatUseCase useCase = new(receiverId, receiverName, receiverZoneName);

        var useCaseHandler = _fixture.GetService<
            IUseCaseHandler<RegisterChatUseCase, RegisterChatUseCaseResponse>
        >();

        Result<RegisterChatUseCaseResponse> result = await useCaseHandler.Handle(useCase);
        Assert.True(result.IsSuccess);

        RegisterChatUseCaseResponse response = result.Value;
        Assert.Equal(receiverId, response.RegisteredChatId);
        Assert.Equal(receiverName, response.RegisteredChatName);

        long themeId = 5;
        RegisterThemeUseCase themeUseCase = new(receiverId, themeId);
        var themeHandler = _fixture.GetService<
            IUseCaseHandler<RegisterThemeUseCase, RegisterThemeResponse>
        >();
        Result<RegisterThemeResponse> themeResult = await themeHandler.Handle(themeUseCase);
        Assert.True(themeResult.IsSuccess);

        const long subjectId = 513;
        const string subjectMessage = "Test Subject Message";
        DateTime dateCreated = DateTime.UtcNow;
        DateTime dateNotify = dateCreated.AddMinutes(1);

        CreateTaskForChatThemeUseCase subjectUseCase = new(
            receiverId,
            themeId,
            subjectId,
            dateCreated,
            dateNotify,
            subjectMessage,
            false
        );
        var subjectHandler = _fixture.GetService<
            IUseCaseHandler<CreateTaskForChatThemeUseCase, CreateTaskForChatThemeUseCaseResponse>
        >();
        Result<CreateTaskForChatThemeUseCaseResponse> responseResult = await subjectHandler.Handle(
            subjectUseCase
        );
        Assert.True(responseResult.IsSuccess);
    }
}
