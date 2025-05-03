using RocketTaskPlanner.Application.Shared.UseCaseHandler;

namespace RocketTaskPlanner.Application.UsersContext.Visitor;

public interface IUsersUseCaseVisitable : IUseCase
{
    Task<Result> Accept(IUsersUseCaseVisitor visitor, CancellationToken ct = default);
}
