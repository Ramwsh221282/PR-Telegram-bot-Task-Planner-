using RocketTaskPlanner.Application.UsersContext.Visitor;

namespace RocketTaskPlanner.Application.UsersContext.Features.AddUser;

public sealed record AddUserUseCase(long UserId, string Username) : IUsersUseCaseVisitable
{
    public async Task<Result> Accept(
        IUsersUseCaseVisitor visitor,
        CancellationToken ct = default
    ) => await visitor.Visit(this, ct);
}
