using RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChatOwner;

/// <summary>
/// Добавить обладателя чата
/// </summary>
/// <param name="Id">ID пользователя</param>
/// <param name="Name">Nick Name пользователя</param>
public sealed record AddExternalChatOwnerUseCase(long Id, string Name)
    : IExternalChatUseCaseVisitable
{
    public async Task<Result> Accept(
        IExternalChatUseCasesVisitor visitor,
        CancellationToken ct = default
    ) => await visitor.Visit(this, ct);
}
