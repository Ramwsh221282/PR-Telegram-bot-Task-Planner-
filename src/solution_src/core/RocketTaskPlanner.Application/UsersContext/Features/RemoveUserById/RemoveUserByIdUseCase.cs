using RocketTaskPlanner.Application.Shared.UseCaseHandler;

namespace RocketTaskPlanner.Application.UsersContext.Features.RemoveUserById;

public sealed record RemoveUserByIdUseCase(long UserId) : IUseCase;
