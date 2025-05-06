using RocketTaskPlanner.Application.NotificationsContext.Features.ChangeTimeZone;
using RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChat;
using RocketTaskPlanner.Application.NotificationsContext.Features.CreateTaskForChatTheme;
using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterChat;
using RocketTaskPlanner.Application.NotificationsContext.Features.RegisterTheme;
using RocketTaskPlanner.Application.NotificationsContext.Features.RemoveChat;
using RocketTaskPlanner.Application.NotificationsContext.Features.RemoveChatSubject;
using RocketTaskPlanner.Application.NotificationsContext.Features.RemoveTheme;
using RocketTaskPlanner.Application.NotificationsContext.Features.RemoveThemeSubject;

namespace RocketTaskPlanner.Application.NotificationsContext.Visitor;

/// <summary>
/// Интерфейс посетителя для выполнения бизнес-логики в контексте чатов и уведомлений
/// </summary>
public interface INotificationUseCaseVisitor
{
    /// <summary>
    /// Создать задачу для чата
    /// </summary>
    /// <param name="useCase">Dto</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Result Success или Failure</returns>
    Task<Result> Visit(CreateTaskForChatUseCase useCase, CancellationToken ct = default);

    /// <summary>
    /// Создать задачу для темы чата
    /// </summary>
    /// <param name="useCase">Dto</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Result Success или Failure</returns>
    Task<Result> Visit(CreateTaskForChatThemeUseCase useCase, CancellationToken ct = default);

    /// <summary>
    /// Создать чат
    /// </summary>
    /// <param name="useCase">Dto</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Result Success или Failure</returns>
    Task<Result> Visit(RegisterChatUseCase useCase, CancellationToken ct = default);

    /// <summary>
    /// Создать тему чата
    /// </summary>
    /// <param name="useCase">Dto</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Result Success или Failure</returns>
    Task<Result> Visit(RegisterThemeUseCase useCase, CancellationToken ct = default);

    /// <summary>
    /// Удалить тему чата
    /// </summary>
    /// <param name="useCase">Dto</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Result Success или Failure</returns>
    Task<Result> Visit(RemoveThemeUseCase useCase, CancellationToken ct = default);

    /// <summary>
    /// Удалить чат получателя
    /// </summary>
    /// <param name="useCase">Dto</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Result Success или Failure</returns>
    Task<Result> Visit(RemoveChatUseCase useCase, CancellationToken ct = default);

    /// <summary>
    /// Изменить временную зону чата уведомлений
    /// </summary>
    /// <param name="useCase">Dto</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Result Success или Failure</returns>
    Task<Result> Visit(ChangeTimeZoneUseCase useCase, CancellationToken ct = default);

    /// <summary>
    /// Удалить задачу основного чата
    /// </summary>
    /// <param name="useCase">Dto</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Result Success или Failure</returns>
    Task<Result> Visit(RemoveChatSubjectUseCase useCase, CancellationToken ct = default);

    /// <summary>
    /// Удалить задачу темы
    /// </summary>
    /// <param name="useCase">Dto</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Result Success или Failure</returns>
    Task<Result> Visit(RemoveThemeSubjectUseCase useCase, CancellationToken ct = default);
}
