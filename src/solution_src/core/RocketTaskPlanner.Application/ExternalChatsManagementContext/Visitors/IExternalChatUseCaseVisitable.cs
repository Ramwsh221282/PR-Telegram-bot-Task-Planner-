using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;

/// <summary>
/// Интерфейс-маркер бизнеслогических операций над объектом <inheritdoc cref="ExternalChatOwner"/>
/// </summary>
public interface IExternalChatUseCaseVisitable : IUseCase
{
    /// <summary>
    /// Обработаться <inheritdoc cref="IExternalChatUseCasesVisitor"/>
    /// </summary>
    Task<Result> Accept(IExternalChatUseCasesVisitor visitor, CancellationToken ct = default);
}
