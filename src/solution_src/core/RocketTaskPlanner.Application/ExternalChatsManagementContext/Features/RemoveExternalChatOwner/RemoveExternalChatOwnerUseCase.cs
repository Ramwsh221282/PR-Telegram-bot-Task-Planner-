using RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChatOwner;

/// <summary>
/// Удалить обладателя чата (пользователя)
/// </summary>
/// <param name="OwnerId">ID обладателя чата</param>
public sealed record RemoveExternalChatOwnerUseCase(long OwnerId) : IExternalChatUseCaseVisitable
{
    public async Task<Result> Accept(
        IExternalChatUseCasesVisitor visitor,
        CancellationToken ct = default
    ) => await visitor.Visit(this, ct);
}
