using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Application.UsersContext.Features.AddUser;
using RocketTaskPlanner.Application.UsersContext.Features.AddUserPermission;
using RocketTaskPlanner.Application.UsersContext.Features.AddUserWithPermissions;
using RocketTaskPlanner.Application.UsersContext.Features.EnsureUserHasPermissions;
using RocketTaskPlanner.Application.UsersContext.Features.RemoveUser;
using RocketTaskPlanner.Application.UsersContext.Features.RemoveUserById;
using RocketTaskPlanner.Domain.UsersContext;
using RocketTaskPlanner.Domain.UsersContext.Entities;

namespace RocketTaskPlanner.Application.UsersContext.Visitor;

public sealed class UsersUseCaseVisitor : IUsersUseCaseVisitor
{
    private readonly IUseCaseHandler<AddUserUseCase, User> _addUser;
    private readonly IUseCaseHandler<AddUserPermissionUseCase, UserPermission> _addUserPermission;
    private readonly IUseCaseHandler<RemoveUserUseCase, User> _removeUser;
    private readonly IUseCaseHandler<RemoveUserByIdUseCase, long> _removeUserById;
    private readonly IUseCaseHandler<AddUserWithPermissionsUseCase, User> _addUserWithPermissions;
    private readonly IUseCaseHandler<EnsureUserHasPermissionsUseCase, User> _userHasPermissions;

    public UsersUseCaseVisitor(
        IUseCaseHandler<AddUserUseCase, User> addUser,
        IUseCaseHandler<AddUserPermissionUseCase, UserPermission> addUserPermission,
        IUseCaseHandler<RemoveUserUseCase, User> removeUser,
        IUseCaseHandler<RemoveUserByIdUseCase, long> removeUserById,
        IUseCaseHandler<AddUserWithPermissionsUseCase, User> addUserWithPermissions,
        IUseCaseHandler<EnsureUserHasPermissionsUseCase, User> userHasPermissions
    )
    {
        _addUser = addUser;
        _addUserPermission = addUserPermission;
        _removeUser = removeUser;
        _removeUserById = removeUserById;
        _addUserWithPermissions = addUserWithPermissions;
        _userHasPermissions = userHasPermissions;
    }

    public async Task<Result> Visit(AddUserUseCase useCase, CancellationToken ct = default) =>
        await _addUser.Handle(useCase, ct);

    public async Task<Result> Visit(
        AddUserPermissionUseCase useCase,
        CancellationToken ct = default
    ) => await _addUserPermission.Handle(useCase, ct);

    public async Task<Result> Visit(RemoveUserUseCase useCase, CancellationToken ct = default) =>
        await _removeUser.Handle(useCase, ct);

    public async Task<Result> Visit(
        RemoveUserByIdUseCase useCase,
        CancellationToken ct = default
    ) => await _removeUserById.Handle(useCase, ct);

    public async Task<Result> Visit(
        AddUserWithPermissionsUseCase useCase,
        CancellationToken ct = default
    ) => await _addUserWithPermissions.Handle(useCase, ct);

    public async Task<Result> Visit(
        EnsureUserHasPermissionsUseCase useCase,
        CancellationToken ct = default
    ) => await _userHasPermissions.Handle(useCase, ct);
}
