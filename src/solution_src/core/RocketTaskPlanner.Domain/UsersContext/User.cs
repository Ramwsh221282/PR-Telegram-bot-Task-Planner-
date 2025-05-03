using RocketTaskPlanner.Domain.UsersContext.Entities;
using RocketTaskPlanner.Domain.UsersContext.ValueObjects;

namespace RocketTaskPlanner.Domain.UsersContext;

/// <summary>
/// Модель пользователя
/// </summary>
public sealed class User
{
    /// <summary>
    /// Права пользователя
    /// </summary>
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

    /// <summary>
    /// Добавление прав пользователю.
    /// </summary>
    /// <param name="permission">Права.</param>
    /// <returns>Success если право было добавлено. Failure если не было добавлено.</returns>
    public Result AddPermission(UserPermission permission)
    {
        if (_permissions.Any(p => p.Id == permission.Id))
            return Result.Failure(
                $"У пользователя {Id.Value} {Name.Value} уже есть права: {permission.Name}"
            );
        _permissions.Add(permission);
        return Result.Success();
    }

    /// <summary>
    /// Проверка на наличие права
    /// </summary>
    /// <param name="permission">Право</param>
    /// <returns>True если право есть, False если права нет</returns>
    public bool HasPermission(string permission) => _permissions.Any(p => p.Name == permission);
}
