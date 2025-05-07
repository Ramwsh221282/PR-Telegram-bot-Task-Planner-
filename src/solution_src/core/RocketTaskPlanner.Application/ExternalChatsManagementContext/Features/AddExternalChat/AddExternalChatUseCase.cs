using RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChat;

/// <summary>
/// Добавить чат
/// <param name="OwnerId">
/// ID обладателя чата
/// </param>
/// <param name="ParentChatId">
/// ID дочернего чата
/// </param>
/// <param name="ChatName">
/// Название чата
/// </param>
/// </summary>
public sealed record AddExternalChatUseCase(long OwnerId, long ParentChatId, string ChatName)
    : IExternalChatUseCaseVisitable
{
    public async Task<Result> Accept(
        IExternalChatUseCasesVisitor visitor,
        CancellationToken ct = default
    ) => await visitor.Visit(this, ct);
}
