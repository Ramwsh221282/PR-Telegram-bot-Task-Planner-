using RocketTaskPlanner.Application.UsersContext.Visitor;

namespace RocketTaskPlanner.Application.UsersContext.Features.AddUserWithPermissions;

public sealed record AddUserWithPermissionsUseCase(
    long UserId,
    string UserName,
    string[] Permissions
) : IUsersUseCaseVisitable
{
    public async Task<Result> Accept(
        IUsersUseCaseVisitor visitor,
        CancellationToken ct = default
    ) => await visitor.Visit(this, ct);
}
