using RocketTaskPlanner.Application.UsersContext.Visitor;
using RocketTaskPlanner.Domain.UsersContext;

namespace RocketTaskPlanner.Application.UsersContext.Features.RemoveUser;

public sealed record RemoveUserUseCase(User user) : IUsersUseCaseVisitable
{
    public async Task<Result> Accept(
        IUsersUseCaseVisitor visitor,
        CancellationToken ct = default
    ) => await visitor.Visit(this, ct);
}
