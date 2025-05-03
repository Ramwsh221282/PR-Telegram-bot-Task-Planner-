using System.Text;
using RocketTaskPlanner.Application.PermissionsContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Application.UsersContext.Contracts;
using RocketTaskPlanner.Domain.PermissionsContext;
using RocketTaskPlanner.Domain.UsersContext;
using RocketTaskPlanner.Domain.UsersContext.Entities;
using RocketTaskPlanner.Domain.UsersContext.ValueObjects;

namespace RocketTaskPlanner.Application.UsersContext.Features.AddUserWithPermissions;

/// <summary>
/// Добавление пользователя с назначением прав одновременно
/// </summary>
public sealed class AddUsersWithPermissionsUseCaseHandler
    : IUseCaseHandler<AddUserWithPermissionsUseCase, User>
{
    private readonly IUsersWritableRepository _users;
    private readonly IPermissionsReadableRepository _permissions;

    public AddUsersWithPermissionsUseCaseHandler(
        IUsersWritableRepository users,
        IPermissionsReadableRepository permissions
    )
    {
        _users = users;
        _permissions = permissions;
    }

    public async Task<Result<User>> Handle(
        AddUserWithPermissionsUseCase useCase,
        CancellationToken ct = default
    )
    {
        List<Permission> permissions = await GetRequiredPermissions(useCase, ct);
        if (permissions.Count == 0)
            return FailurePermissionsResult(useCase);

        UserId userId = UserId.Create(useCase.UserId);
        Result<UserName> userName = UserName.Create(useCase.UserName);
        User user = new(userId, userName.Value);

        Result<List<UserPermission>> userPermissions = AddUserPermissions(user, permissions);
        if (userPermissions.IsFailure)
            return Result.Failure<User>(userPermissions.Error);

        _users.BeginTransaction();
        _users.AddUser(user, ct);
        foreach (UserPermission userPermission in userPermissions.Value)
            _users.AddUserPermission(userPermission, ct);
        Result saving = await _users.Save();

        if (saving.IsFailure)
            return Result.Failure<User>(
                $"Не удалось добавить пользователя: {useCase.UserId} {useCase.UserName} с правами: {string.Join(' ', useCase.Permissions)}"
            );
        return user;
    }

    private async Task<List<Permission>> GetRequiredPermissions(
        AddUserWithPermissionsUseCase useCase,
        CancellationToken ct
    )
    {
        List<Permission> permissions = [];

        foreach (string permissionName in useCase.Permissions)
        {
            Result<Permission> permission = await _permissions.GetByName(permissionName, ct);
            if (permission.IsFailure)
                continue;
            permissions.Add(permission.Value);
        }

        return permissions;
    }

    private static Result<List<UserPermission>> AddUserPermissions(
        User user,
        List<Permission> permissions
    )
    {
        List<UserPermission> userPermissions = [];

        foreach (Permission permission in permissions)
        {
            string permissionName = permission.Name;
            Guid permissionId = permission.Id;
            UserPermission userPermisstion = new(user, permissionName, permissionId);
            Result addingPermission = user.AddPermission(userPermisstion);
            if (addingPermission.IsFailure)
                return Result.Failure<List<UserPermission>>(addingPermission.Error);
            userPermissions.Add(userPermisstion);
        }

        return userPermissions;
    }

    private static Result<User> FailurePermissionsResult(AddUserWithPermissionsUseCase useCase)
    {
        StringBuilder sb = new();
        sb.Append($"Для добавления пользователя: {useCase.UserName} {useCase.UserId}");
        sb.AppendLine("Следующих прав не существует: ");
        foreach (string permission in useCase.Permissions)
            sb.AppendLine(permission);
        return Result.Failure<User>(sb.ToString());
    }
}
