using RocketTaskPlanner.Application.Shared.UseCaseHandler;

namespace RocketTaskPlanner.Application.UsersContext.Features.AddUserPermission;

public sealed record AddUserPermissionUseCase(long UserId, Guid PermissionId, string PermissionName)
    : IUseCase;
