using RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChat;

/// <summary>
/// Удалить чат обладателя
/// </summary>
/// <param name="ownerId">ID обладателя чата</param>
/// <param name="chatId">ID чата</param>
public sealed record RemoveExternalChatUseCase(long ownerId, long chatId)
    : IExternalChatUseCaseVisitable
{
    public async Task<Result> Accept(
        IExternalChatUseCasesVisitor visitor,
        CancellationToken ct = default
    ) => await visitor.Visit(this, ct);
}
