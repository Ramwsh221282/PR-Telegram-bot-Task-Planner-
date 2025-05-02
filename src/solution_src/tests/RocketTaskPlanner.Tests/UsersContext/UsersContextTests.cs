using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.PermissionsContext.Features.AddPermission;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Application.UsersContext.Contracts;
using RocketTaskPlanner.Application.UsersContext.Features.AddUser;
using RocketTaskPlanner.Application.UsersContext.Features.AddUserPermission;
using RocketTaskPlanner.Domain.PermissionsContext;
using RocketTaskPlanner.Domain.UsersContext;
using RocketTaskPlanner.Domain.UsersContext.Entities;
using RocketTaskPlanner.Domain.UsersContext.ValueObjects;
using RocketTaskPlanner.Tests.TestDependencies;

namespace RocketTaskPlanner.Tests.UsersContext;

public sealed class UsersContextTests : IClassFixture<DefaultTestsFixture>
{
    private readonly DefaultTestsFixture _fixture;

    public UsersContextTests(DefaultTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Add_User_Success()
    {
        const long userId = 123;
        const string userName = "USER";
        AddUserUseCase useCase = new(userId, userName);
        var handler = _fixture.GetService<IUseCaseHandler<AddUserUseCase, User>>();
        Result<User> userResult = await handler.Handle(useCase);
        Assert.True(userResult.IsSuccess);
        User user = userResult.Value;
        Assert.Equal(userId, user.Id.Value);
        Assert.Equal(userName, user.Name.Value);
    }

    [Fact]
    public async Task Add_UserAndEnsureCreated_Success()
    {
        const long userId = 123;
        const string userName = "USER";
        AddUserUseCase useCase = new(userId, userName);
        var handler = _fixture.GetService<IUseCaseHandler<AddUserUseCase, User>>();
        Result<User> userResult = await handler.Handle(useCase);

        Assert.True(userResult.IsSuccess);
        User user = userResult.Value;
        Assert.Equal(userId, user.Id.Value);
        Assert.Equal(userName, user.Name.Value);

        IUsersReadableRepository repository = _fixture.GetService<IUsersReadableRepository>();
        UserId userIdVO = UserId.Create(userId);
        Result<User> created = await repository.GetById(userIdVO);

        Assert.True(created.IsSuccess);
        Assert.Equal(user.Id, created.Value.Id);
        Assert.Equal(user.Name, created.Value.Name);
    }

    [Fact]
    public async Task Add_UserEditorPermission_Success()
    {
        const long userId = 123;
        const string userName = "USER";
        const string permissionName = PermissionNames.EditConfiguration;

        AddPermissionUseCase permUseCase = new(permissionName);
        var permUseCaseHandler = _fixture.GetService<
            IUseCaseHandler<AddPermissionUseCase, Permission>
        >();
        Result<Permission> permResult = await permUseCaseHandler.Handle(permUseCase);
        Assert.True(permResult.IsSuccess);

        AddUserUseCase addUsrUseCase = new(userId, userName);
        var usrHandler = _fixture.GetService<IUseCaseHandler<AddUserUseCase, User>>();
        Result<User> userResult = await usrHandler.Handle(addUsrUseCase);
        Assert.True(userResult.IsSuccess);

        AddUserPermissionUseCase usrPermUseCase = new(userId, permResult.Value.Id, permissionName);
        var usrPermUseCaseHandler = _fixture.GetService<
            IUseCaseHandler<AddUserPermissionUseCase, UserPermission>
        >();
        Result<UserPermission> usrPermResult = await usrPermUseCaseHandler.Handle(usrPermUseCase);
        Assert.True(usrPermResult.IsSuccess);

        UserPermission userPermission = usrPermResult.Value;
        User user = userResult.Value;
        Permission permission = permResult.Value;

        Assert.Equal(user.Id, userPermission.UserId);
        Assert.Equal(user.Name, userPermission.User.Name);
        Assert.Equal(permission.Name, userPermission.Name);
        Assert.Equal(permission.Id, userPermission.Id);
    }

    [Fact]
    public async Task Add_UserPermissionDuplicate_Failure()
    {
        const long userId = 123;
        const string userName = "USER";
        const string permissionName = PermissionNames.EditConfiguration;

        AddPermissionUseCase permUseCase = new(permissionName);
        var permUseCaseHandler = _fixture.GetService<
            IUseCaseHandler<AddPermissionUseCase, Permission>
        >();
        Result<Permission> permResult = await permUseCaseHandler.Handle(permUseCase);
        Assert.True(permResult.IsSuccess);

        AddUserUseCase addUsrUseCase = new(userId, userName);
        var usrHandler = _fixture.GetService<IUseCaseHandler<AddUserUseCase, User>>();
        Result<User> userResult = await usrHandler.Handle(addUsrUseCase);
        Assert.True(userResult.IsSuccess);

        AddUserPermissionUseCase usrPermUseCase = new(userId, permResult.Value.Id, permissionName);
        var usrPermUseCaseHandler = _fixture.GetService<
            IUseCaseHandler<AddUserPermissionUseCase, UserPermission>
        >();
        Result<UserPermission> usrPermResult = await usrPermUseCaseHandler.Handle(usrPermUseCase);
        Assert.True(usrPermResult.IsSuccess);

        UserPermission userPermission = usrPermResult.Value;
        User user = userResult.Value;
        Permission permission = permResult.Value;

        Assert.Equal(user.Id, userPermission.UserId);
        Assert.Equal(user.Name, userPermission.User.Name);
        Assert.Equal(permission.Name, userPermission.Name);
        Assert.Equal(permission.Id, userPermission.Id);

        Result<UserPermission> usrPermResultDuplicate = await usrPermUseCaseHandler.Handle(
            usrPermUseCase
        );
        Assert.True(usrPermResultDuplicate.IsFailure);
    }
}
