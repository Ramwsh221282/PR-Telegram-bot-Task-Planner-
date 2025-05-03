using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Application.UsersContext.Contracts;
using RocketTaskPlanner.Domain.UsersContext;
using RocketTaskPlanner.Domain.UsersContext.ValueObjects;

namespace RocketTaskPlanner.Application.UsersContext.Features.EnsureUserHasPermissions;

/// <summary>
/// Проверка наличия прав у пользователя
/// </summary>
public sealed class EnsureUserHasPermissionsUseCaseHandler
    : IUseCaseHandler<EnsureUserHasPermissionsUseCase, User>
{
    private readonly IUsersReadableRepository _repository;

    public EnsureUserHasPermissionsUseCaseHandler(IUsersReadableRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<User>> Handle(
        EnsureUserHasPermissionsUseCase useCase,
        CancellationToken ct = default
    )
    {
        UserId userId = UserId.Create(useCase.UserId);
        Result<User> userResult = await _repository.GetById(userId, ct);
        if (userResult.IsFailure)
            return userResult;

        User user = userResult.Value;
        bool hasPermissions = useCase.Permissions.All(p => user.HasPermission(p));

        return !hasPermissions
            ? Result.Failure<User>($"Нет прав: {string.Join(' ', useCase.Permissions)}")
            : user;
    }
}
