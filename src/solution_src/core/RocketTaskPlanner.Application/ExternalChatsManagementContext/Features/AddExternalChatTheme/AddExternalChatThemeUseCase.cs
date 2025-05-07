using RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChatTheme;

/// <summary>
/// Добавить дочерний чат (тему)
/// </summary>
/// <param name="ParentChatId">ID родителя (основного чата)</param>
/// <param name="ThemeId">ID дочернего чата (темы)</param>
/// <param name="OwnerId">ID обладателя основного чата</param>
/// <param name="ChatName">Название дочернего чата</param>
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
