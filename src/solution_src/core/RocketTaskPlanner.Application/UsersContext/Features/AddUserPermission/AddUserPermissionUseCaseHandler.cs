using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Application.UsersContext.Contracts;
using RocketTaskPlanner.Domain.UsersContext;
using RocketTaskPlanner.Domain.UsersContext.Entities;
using RocketTaskPlanner.Domain.UsersContext.ValueObjects;

namespace RocketTaskPlanner.Application.UsersContext.Features.AddUserPermission;

public sealed class AddUserPermissionUseCaseHandler
    : IUseCaseHandler<AddUserPermissionUseCase, UserPermission>
{
    private readonly IUsersRepository _repository;

    public AddUserPermissionUseCaseHandler(IUsersRepository repository) => _repository = repository;

    public async Task<Result<UserPermission>> Handle(
        AddUserPermissionUseCase useCase,
        CancellationToken ct = default
    )
    {
        UserId userId = UserId.Create(useCase.UserId);
        Result<User> userResult = await _repository.ReadableRepository.GetById(userId, ct);
        if (userResult.IsFailure)
            return Result.Failure<UserPermission>(userResult.Error);

        User user = userResult.Value;
        string permissionName = useCase.PermissionName;
        Guid permissionId = useCase.PermissionId;
        UserPermission permission = new(user, permissionName, permissionId);
        Result addingPermission = user.AddPermission(permission);
        if (addingPermission.IsFailure)
            return Result.Failure<UserPermission>(addingPermission.Error);

        _repository.WritableRepository.BeginTransaction();
        _repository.WritableRepository.AddUserPermission(permission, ct);
        Result saving = await _repository.WritableRepository.Save();

        return saving.IsFailure
            ? await Task.FromResult(Result.Failure<UserPermission>(saving.Error))
            : await Task.FromResult(permission);
    }
}
