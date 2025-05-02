using RocketTaskPlanner.Application.Shared.UseCaseHandler;

namespace RocketTaskPlanner.Application.UsersContext.Features.AddUser;

public sealed record AddUserUseCase(long UserId, string Username) : IUseCase;
