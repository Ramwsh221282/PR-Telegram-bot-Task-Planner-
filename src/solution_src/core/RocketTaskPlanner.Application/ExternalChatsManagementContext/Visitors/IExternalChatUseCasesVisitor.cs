using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChat;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChatOwner;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChatTheme;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChat;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChatOwner;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChatTheme;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;

public interface IExternalChatUseCasesVisitor
{
    /// <summary>
    /// Добавление внешнего чата обладателю внешнего чата.
    /// </summary>
    /// <param name="useCase">Dto с данными для добавления внешнего чата</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат Success или Failure</returns>
    Task<Result> Visit(AddExternalChatUseCase useCase, CancellationToken ct = default);

    /// <summary>
    /// Добавление обладателя внешнего чата.
    /// </summary>
    /// <param name="useCase">Dto с данными для добавления внешнего чата</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат Success или Failure</returns>
    Task<Result> Visit(AddExternalChatOwnerUseCase useCase, CancellationToken ct = default);

    /// <summary>
    /// Удаление внешнего чата у обладателя.
    /// </summary>
    /// <param name="useCase">Dto с данными для добавления внешнего чата</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат Success или Failure</returns>
    Task<Result> Visit(RemoveExternalChatUseCase useCase, CancellationToken ct = default);

    /// <summary>
    /// Удаление обладателя внешнего чата.
    /// </summary>
    /// <param name="useCase">Dto с данными для удаления внешнего чата</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат Success или Failure</returns>
    Task<Result> Visit(RemoveExternalChatOwnerUseCase useCase, CancellationToken ct = default);

    /// <summary>
    /// Добавление внешнего дочернего чата(темы) в качестве чата обладателя
    /// </summary>
    /// <param name="useCase">Dto</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат Success или Failure</returns>
    Task<Result> Visit(AddExternalChatThemeUseCase useCase, CancellationToken ct = default);

    /// <summary>
    /// Удаление дочернего чата у обладателя.
    /// </summary>
    /// <param name="useCase">Dto</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат Success или Failure</returns>
    Task<Result> Visit(RemoveExternalChatThemeUseCase useCase, CancellationToken ct = default);
}
