using RocketTaskPlanner.Application.UsersContext.Visitor;

namespace RocketTaskPlanner.Application.UsersContext.Features.AddUserPermission;

public sealed record AddUserPermissionUseCase(long UserId, Guid PermissionId, string PermissionName)
    : IUsersUseCaseVisitable
{
    public async Task<Result> Accept(
        IUsersUseCaseVisitor visitor,
        CancellationToken ct = default
    ) => await visitor.Visit(this, ct);
}
