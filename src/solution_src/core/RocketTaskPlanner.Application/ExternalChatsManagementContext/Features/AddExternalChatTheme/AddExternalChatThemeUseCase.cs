using RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChatTheme;

public sealed record AddExternalChatThemeUseCase(
    long ParentChatId,
    long ThemeId,
    long OwnerId,
    string ChatName
) : IExternalChatUseCaseVisitable
{
    public async Task<Result> Accept(
        IExternalChatUseCasesVisitor visitor,
        CancellationToken ct = default
    ) => await visitor.Visit(this, ct);
}
