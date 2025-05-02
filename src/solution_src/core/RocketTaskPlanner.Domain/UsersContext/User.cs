using RocketTaskPlanner.Domain.UsersContext.Entities;
using RocketTaskPlanner.Domain.UsersContext.ValueObjects;

namespace RocketTaskPlanner.Domain.UsersContext;

public sealed class User
{
    private readonly List<UserPermission> _permissions = [];
    public UserId Id { get; }
    public UserName Name { get; } = null!;

    public IReadOnlyCollection<UserPermission> Permissions => _permissions;

    private User() { } // ef core

    public User(UserId id, UserName name)
    {
        Id = id;
        Name = name;
    }

    public Result AddPermission(UserPermission permission)
    {
        if (_permissions.Any(p => p.Id == permission.Id))
            return Result.Failure(
                $"У пользователя {Id.Value} {Name.Value} уже есть права: {permission.Name}"
            );
        _permissions.Add(permission);
        return Result.Success();
    }

    public bool HasPermission(string permission) => _permissions.Any(p => p.Name == permission);
}
