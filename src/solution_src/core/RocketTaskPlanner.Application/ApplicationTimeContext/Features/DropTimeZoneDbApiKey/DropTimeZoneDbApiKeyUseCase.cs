using RocketTaskPlanner.Application.Shared.UseCaseHandler;

namespace RocketTaskPlanner.Application.ApplicationTimeContext.Features.DropTimeZoneDbApiKey;

public sealed record DropTimeZoneDbApiKeyUseCase(string Message) : IUseCase;
