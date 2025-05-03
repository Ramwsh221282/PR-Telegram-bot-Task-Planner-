using RocketTaskPlanner.Application.UsersContext.Features.AddUser;
using RocketTaskPlanner.Application.UsersContext.Features.AddUserPermission;
using RocketTaskPlanner.Application.UsersContext.Features.AddUserWithPermissions;
using RocketTaskPlanner.Application.UsersContext.Features.EnsureUserHasPermissions;
using RocketTaskPlanner.Application.UsersContext.Features.RemoveUser;
using RocketTaskPlanner.Application.UsersContext.Features.RemoveUserById;

namespace RocketTaskPlanner.Application.UsersContext.Visitor;

public interface IUsersUseCaseVisitor
{
    Task<Result> Visit(AddUserUseCase useCase, CancellationToken ct = default);
    Task<Result> Visit(AddUserPermissionUseCase useCase, CancellationToken ct = default);
    Task<Result> Visit(RemoveUserUseCase useCase, CancellationToken ct = default);
    Task<Result> Visit(RemoveUserByIdUseCase useCase, CancellationToken ct = default);
    Task<Result> Visit(AddUserWithPermissionsUseCase useCase, CancellationToken ct = default);
    Task<Result> Visit(EnsureUserHasPermissionsUseCase useCase, CancellationToken ct = default);
}
