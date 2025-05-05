using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChat;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChatOwner;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChat;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext;
using RocketTaskPlanner.Tests.TestDependencies;

namespace RocketTaskPlanner.Tests.ExternalChatContext;

public sealed class ExternalChatContextTests : IClassFixture<DefaultTestsFixture>
{
    private readonly DefaultTestsFixture _fixture;
    private readonly IExternalChatUseCasesVisitor _visitor;
    private readonly IExternalChatsReadableRepository _repository;

    public ExternalChatContextTests(DefaultTestsFixture fixture)
    {
        _fixture = fixture;
        _visitor = fixture.GetService<IExternalChatUseCasesVisitor>();
        _repository = fixture.GetService<IExternalChatsReadableRepository>();
    }

    [Fact]
    public async Task Add_External_Chat_Owner_And_Ensure_Created_Success()
    {
        const long ownerId = 123;
        const string ownerName = "Owner Name";
        AddExternalChatOwnerUseCase useCase = new(ownerId, ownerName);
        Result adding = await useCase.Accept(_visitor);
        Assert.True(adding.IsSuccess);

        Result<ExternalChatOwner> created = await _repository.GetExternalChatOwnerById(ownerId);
        Assert.True(created.IsSuccess);
        Assert.Equal(ownerId, created.Value.Id.Value);
        Assert.Equal(ownerName, created.Value.Name.Value);
    }

    [Fact]
    public async Task Add_External_Chat_Owner_And_External_Chat_Success()
    {
        const long ownerId = 123;
        const string ownerName = "Owner Name";
        AddExternalChatOwnerUseCase addOwnerUseCase = new(ownerId, ownerName);
        Result adding = await addOwnerUseCase.Accept(_visitor);
        Assert.True(adding.IsSuccess);

        Result<ExternalChatOwner> created = await _repository.GetExternalChatOwnerById(ownerId);
        Assert.True(created.IsSuccess);
        Assert.Equal(ownerId, created.Value.Id.Value);
        Assert.Equal(ownerName, created.Value.Name.Value);

        const long chatId = 123;
        const string chatName = "Chat Name";
        AddExternalChatUseCase addChatUseCase = new(ownerId, chatId, chatName);
        Result addingChat = await addChatUseCase.Accept(_visitor);
        Assert.True(addingChat.IsSuccess);

        created = await _repository.GetExternalChatOwnerById(chatId);
        Assert.True(created.IsSuccess);
        Assert.Equal(ownerId, created.Value.Id.Value);
        Assert.Equal(ownerName, created.Value.Name.Value);
        Assert.Contains(created.Value.Chats, c => c.Id.Value == chatId && c.Name.Value == chatName);
    }

    [Fact]
    public async Task Add_ExternalChatOwner_And_ExternalChat_And_RemoveExternalChat_Success()
    {
        const long ownerId = 123;
        const string ownerName = "Owner Name";
        AddExternalChatOwnerUseCase addOwnerUseCase = new(ownerId, ownerName);
        Result adding = await addOwnerUseCase.Accept(_visitor);
        Assert.True(adding.IsSuccess);

        Result<ExternalChatOwner> created = await _repository.GetExternalChatOwnerById(ownerId);
        Assert.True(created.IsSuccess);
        Assert.Equal(ownerId, created.Value.Id.Value);
        Assert.Equal(ownerName, created.Value.Name.Value);

        const long chatId = 123;
        const string chatName = "Chat Name";
        AddExternalChatUseCase addChatUseCase = new(ownerId, chatId, chatName);
        Result addingChat = await addChatUseCase.Accept(_visitor);
        Assert.True(addingChat.IsSuccess);

        created = await _repository.GetExternalChatOwnerById(chatId);
        Assert.True(created.IsSuccess);
        Assert.Equal(ownerId, created.Value.Id.Value);
        Assert.Equal(ownerName, created.Value.Name.Value);
        Assert.Contains(created.Value.Chats, c => c.Id.Value == chatId && c.Name.Value == chatName);

        RemoveExternalChatUseCase removeChat = new(ownerId, chatId);
        Result removing = await removeChat.Accept(_visitor);
        Assert.True(removing.IsSuccess);

        created = await _repository.GetExternalChatOwnerById(chatId);
        Assert.True(created.IsSuccess);
        Assert.Equal(ownerId, created.Value.Id.Value);
        Assert.Equal(ownerName, created.Value.Name.Value);
        Assert.Empty(created.Value.Chats);
    }
}
