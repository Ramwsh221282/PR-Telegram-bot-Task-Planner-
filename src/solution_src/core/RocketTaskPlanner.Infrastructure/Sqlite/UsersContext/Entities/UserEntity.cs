using CSharpFunctionalExtensions;
using RocketTaskPlanner.Domain.UsersContext;
using RocketTaskPlanner.Domain.UsersContext.Entities;
using RocketTaskPlanner.Domain.UsersContext.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.Sqlite.UsersContext.Entities;

/// <summary>
/// Dao модель пользователя
/// </summary>
public sealed class UserEntity
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<UserPermissionEntity> Permissions { get; set; } = [];

    public UserEntity() { } //

    public UserEntity(UserEntity user)
    {
        Id = user.Id;
        Name = user.Name;
    }

    public void TryAddPermission(UserPermissionEntity permission)
    {
        permission.UserId = Id;
        if (Permissions.Any(p => p.Id == permission.Id && p.UserId == permission.UserId))
        {
            permission.UserId = -1;
            return;
        }
        Permissions.Add(permission);
    }

    public User ConvertToUser()
    {
        UserId id = UserId.Create(Id);
        Result<UserName> name = UserName.Create(Name);
        User user = new(id, name.Value);
        foreach (UserPermissionEntity entry in Permissions)
        {
            string permissionName = entry.Name;
            Guid permissionId = Guid.Parse(entry.Id);
            UserPermission permission = new(user, permissionName, permissionId);
            user.AddPermission(permission);
        }
        return user;
    }
}
