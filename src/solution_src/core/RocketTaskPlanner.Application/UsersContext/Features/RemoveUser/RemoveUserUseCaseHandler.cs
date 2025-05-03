using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Application.UsersContext.Contracts;
using RocketTaskPlanner.Domain.UsersContext;

namespace RocketTaskPlanner.Application.UsersContext.Features.RemoveUser;

/// <summary>
/// Удаление пользователя
/// </summary>
public sealed class RemoveUserUseCaseHandler : IUseCaseHandler<RemoveUserUseCase, User>
{
    private readonly IUsersWritableRepository _repository;

    public RemoveUserUseCaseHandler(IUsersWritableRepository repository) =>
        _repository = repository;

    public async Task<Result<User>> Handle(
        RemoveUserUseCase useCase,
        CancellationToken ct = default
    )
    {
        User user = useCase.user;
        _repository.BeginTransaction();
        _repository.RemoveUser(user, ct);
        Result saving = await _repository.Save();

        return saving.IsFailure
            ? Result.Failure<User>(
                $"Не удалось удалить пользователя: {user.Id.Value} {user.Name.Value}"
            )
            : user;
    }
}
