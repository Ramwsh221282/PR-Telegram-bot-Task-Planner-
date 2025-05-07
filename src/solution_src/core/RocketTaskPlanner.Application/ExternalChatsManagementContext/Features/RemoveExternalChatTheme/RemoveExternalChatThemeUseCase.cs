using RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChatTheme;

/// <summary>
/// Удалить дочерний чат основного чата
/// </summary>
/// <param name="userId">ID обладателя основного чата</param>
/// <param name="chatId">ID основного чата</param>
/// <param name="themeId">ID дочернего чата</param>
public sealed record RemoveExternalChatThemeUseCase(long userId, long chatId, long themeId)
    : IExternalChatUseCaseVisitable
{
    public async Task<Result> Accept(
        IExternalChatUseCasesVisitor visitor,
        CancellationToken ct = default
    ) => await visitor.Visit(this, ct);
}
