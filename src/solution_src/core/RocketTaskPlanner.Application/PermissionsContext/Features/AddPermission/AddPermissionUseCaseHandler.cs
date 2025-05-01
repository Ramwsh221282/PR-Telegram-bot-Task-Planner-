using RocketTaskPlanner.Application.PermissionsContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.PermissionsContext;

namespace RocketTaskPlanner.Application.PermissionsContext.Features.AddPermission;

public sealed class AddPermissionUseCaseHandler(IPermissionsRepository repository)
    : IUseCaseHandler<AddPermissionUseCase, Permission>
{
    private readonly IPermissionsRepository _repository = repository;

    public async Task<Result<Permission>> Handle(
        AddPermissionUseCase useCase,
        CancellationToken ct = default
    )
    {
        string name = useCase.PermissionName;
        Guid id = Guid.NewGuid();
        Result<Permission> existing = await _repository.GetByName(name, ct);
        if (existing.IsSuccess)
            return Result.Failure<Permission>($"Права: {name} уже существуют.");
        Permission permission = new() { Id = id, Name = name };
        await _repository.Add(permission, ct);
        return permission;
    }
}
