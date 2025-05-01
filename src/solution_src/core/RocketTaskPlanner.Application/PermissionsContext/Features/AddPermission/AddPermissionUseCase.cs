using RocketTaskPlanner.Application.Shared.UseCaseHandler;

namespace RocketTaskPlanner.Application.PermissionsContext.Features.AddPermission;

public sealed record AddPermissionUseCase(string PermissionName) : IUseCase;
