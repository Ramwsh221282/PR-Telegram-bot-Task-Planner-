using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Application.UsersContext.Contracts;

namespace RocketTaskPlanner.Application.UsersContext.Features.RemoveUserById;

/// <summary>
/// Удаление пользователя по ID
/// </summary>
public sealed class RemoveUserByIdUseCaseHandler : IUseCaseHandler<RemoveUserByIdUseCase, long>
{
    private readonly IUsersWritableRepository _repository;

    public RemoveUserByIdUseCaseHandler(IUsersWritableRepository repository) =>
        _repository = repository;

    public async Task<Result<long>> Handle(
        RemoveUserByIdUseCase useCase,
        CancellationToken ct = default
    )
    {
        long userId = useCase.UserId;
        _repository.BeginTransaction();
        _repository.RemoveUser(userId, ct);
        Result saving = await _repository.Save();

        return saving.IsFailure
            ? Result.Failure<long>($"Не удалось удалить пользователя с ID: {userId}")
            : userId;
    }
}
