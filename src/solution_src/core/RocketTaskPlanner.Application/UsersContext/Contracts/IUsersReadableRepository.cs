using RocketTaskPlanner.Domain.UsersContext;
using RocketTaskPlanner.Domain.UsersContext.ValueObjects;

namespace RocketTaskPlanner.Application.UsersContext.Contracts;

/// <summary>
/// Контракт взаимодействия с БД (чтение)
/// </summary>
public interface IUsersReadableRepository
{
    Task<Result<User>> GetById(UserId id, CancellationToken ct = default);
    Task<bool> Exists(UserId id, CancellationToken ct = default);
    Task<bool> ContainsOwner(CancellationToken ct = default);
}
