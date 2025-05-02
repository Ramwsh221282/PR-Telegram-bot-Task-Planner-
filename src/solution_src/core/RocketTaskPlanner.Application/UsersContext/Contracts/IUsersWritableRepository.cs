using RocketTaskPlanner.Domain.UsersContext;
using RocketTaskPlanner.Domain.UsersContext.Entities;

namespace RocketTaskPlanner.Application.UsersContext.Contracts;

public interface IUsersWritableRepository
{
    public void BeginTransaction();

    public Result<User> AddUser(User user, CancellationToken ct = default);

    public Result<UserPermission> AddUserPermission(
        UserPermission permission,
        CancellationToken ct = default
    );

    public Task<Result> Save();
}
