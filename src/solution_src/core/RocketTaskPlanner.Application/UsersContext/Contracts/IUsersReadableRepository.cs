using RocketTaskPlanner.Domain.UsersContext;
using RocketTaskPlanner.Domain.UsersContext.ValueObjects;

namespace RocketTaskPlanner.Application.UsersContext.Contracts;

public interface IUsersReadableRepository
{
    public Task<Result<User>> GetById(UserId id, CancellationToken ct = default);
    public Task<bool> Exists(UserId id, CancellationToken ct = default);
}
