using RocketTaskPlanner.Application.Shared.UseCaseHandler;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;

public interface IExternalChatUseCaseVisitable : IUseCase
{
    Task<Result> Accept(IExternalChatUseCasesVisitor visitor, CancellationToken ct = default);
}
