using RocketTaskPlanner.Application.Shared.UseCaseHandler;

namespace RocketTaskPlanner.Application.ApplicationTimeContext.Features.SaveTimeZoneDbApiKey;

public sealed record SaveTimeZoneDbApiKeyUseCase(string ApiKey) : IUseCase;
