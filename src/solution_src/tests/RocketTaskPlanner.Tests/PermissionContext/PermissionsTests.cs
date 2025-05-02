using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.PermissionsContext.Features.AddPermission;
using RocketTaskPlanner.Application.PermissionsContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.PermissionsContext;
using RocketTaskPlanner.Tests.TestDependencies;

namespace RocketTaskPlanner.Tests.PermissionContext;

public sealed class PermissionsTests : IClassFixture<DefaultTestsFixture>
{
    private readonly DefaultTestsFixture _fixture;

    public PermissionsTests(DefaultTestsFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Add_Permission_Success()
    {
        const string permissionName = PermissionNames.EditConfiguration;
        AddPermissionUseCase useCase = new(permissionName);
        var useCaseHandler = _fixture.GetService<
            IUseCaseHandler<AddPermissionUseCase, Permission>
        >();
        Result<Permission> result = await useCaseHandler.Handle(useCase);
        Assert.True(result.IsSuccess);
        Assert.Equal(permissionName, result.Value.Name);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
    }

    [Fact]
    public async Task Add_PermissionAndEnsureCreated_Success()
    {
        const string permissionName = PermissionNames.EditConfiguration;
        AddPermissionUseCase useCase = new(permissionName);
        var useCaseHandler = _fixture.GetService<
            IUseCaseHandler<AddPermissionUseCase, Permission>
        >();
        Result<Permission> result = await useCaseHandler.Handle(useCase);
        Assert.True(result.IsSuccess);
        Assert.Equal(permissionName, result.Value.Name);
        Assert.NotEqual(Guid.Empty, result.Value.Id);

        IPermissionsReadableRepository repository =
            _fixture.GetService<IPermissionsReadableRepository>();
        Result<Permission> created = await repository.GetByName(permissionName);
        Assert.True(created.IsSuccess);
        Assert.Equal(permissionName, created.Value.Name);
        Assert.Equal(result.Value.Id, created.Value.Id);
    }

    [Fact]
    public async Task Add_DuplicatePermission_Failure()
    {
        const string permissionName = PermissionNames.EditConfiguration;
        AddPermissionUseCase useCase = new(permissionName);
        var useCaseHandler = _fixture.GetService<
            IUseCaseHandler<AddPermissionUseCase, Permission>
        >();
        Result<Permission> result = await useCaseHandler.Handle(useCase);
        Assert.True(result.IsSuccess);
        Assert.Equal(permissionName, result.Value.Name);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Result<Permission> duplicate = await useCaseHandler.Handle(useCase);
        Assert.False(duplicate.IsSuccess);
    }
}
