using RocketTaskPlanner.Application.UsersContext.Visitor;

namespace RocketTaskPlanner.Application.UsersContext.Features.EnsureUserHasPermissions;

public sealed record EnsureUserHasPermissionsUseCase(long UserId, string[] Permissions)
    : IUsersUseCaseVisitable
{
    public async Task<Result> Accept(
        IUsersUseCaseVisitor visitor,
        CancellationToken ct = default
    ) => await visitor.Visit(this, ct);
}
