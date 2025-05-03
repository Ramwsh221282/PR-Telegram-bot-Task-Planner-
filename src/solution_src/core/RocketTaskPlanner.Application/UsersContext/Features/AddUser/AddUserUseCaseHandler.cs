using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Application.UsersContext.Contracts;
using RocketTaskPlanner.Domain.UsersContext;
using RocketTaskPlanner.Domain.UsersContext.ValueObjects;

namespace RocketTaskPlanner.Application.UsersContext.Features.AddUser;

/// <summary>
/// Обработчик добавления пользователя
/// </summary>
public sealed class AddUserUseCaseHandler : IUseCaseHandler<AddUserUseCase, User>
{
    private readonly IUsersWritableRepository _writableRepository;

    public AddUserUseCaseHandler(IUsersWritableRepository writableRepository)
    {
        _writableRepository = writableRepository;
    }

    public async Task<Result<User>> Handle(AddUserUseCase useCase, CancellationToken ct = default)
    {
        Result<UserName> name = UserName.Create(useCase.Username);
        UserId id = UserId.Create(useCase.UserId);
        User user = new(id, name.Value);

        _writableRepository.BeginTransaction();
        _writableRepository.AddUser(user, ct);
        Result saving = await _writableRepository.Save();

        return saving.IsFailure
            ? Result.Failure<User>($"Не удалось создать пользователя: {id.Value} {name.Value}")
            : user;
    }
}
