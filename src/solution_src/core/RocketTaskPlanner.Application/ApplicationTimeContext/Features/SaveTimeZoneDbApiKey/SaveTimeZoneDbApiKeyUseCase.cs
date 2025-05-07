using RocketTaskPlanner.Application.Shared.UseCaseHandler;

namespace RocketTaskPlanner.Application.ApplicationTimeContext.Features.SaveTimeZoneDbApiKey;

/// <summary>
/// Добавить провайдера временных зон в приложение
/// <param name="ApiKey">Токен</param>
/// </summary>
public sealed record SaveTimeZoneDbApiKeyUseCase(string ApiKey) : IUseCase;
