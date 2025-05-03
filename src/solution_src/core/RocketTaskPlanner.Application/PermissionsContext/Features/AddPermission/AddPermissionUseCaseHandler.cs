using RocketTaskPlanner.Application.PermissionsContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.PermissionsContext;

namespace RocketTaskPlanner.Application.PermissionsContext.Features.AddPermission;

/// <summary>
/// Создание права
/// </summary>
/// <param name="repository">Контракт взаимодействия с БД</param>
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
        bool containsPermission = await _repository.ReadableRepository.Contains(name, ct);
        if (containsPermission)
            return Result.Failure<Permission>($"Права: {name} уже существуют.");

        Guid id = Guid.NewGuid();
        Permission permission = new() { Id = id, Name = name };
        permission = await _repository.WritableRepository.Add(permission, ct);

        return permission;
    }
}
