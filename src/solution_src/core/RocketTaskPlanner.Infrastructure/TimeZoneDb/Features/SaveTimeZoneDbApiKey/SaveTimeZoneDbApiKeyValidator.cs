using RocketTaskPlanner.Application.ApplicationTimeContext.Features.SaveTimeZoneDbApiKey;
using RocketTaskPlanner.Application.Shared.Validation;

namespace RocketTaskPlanner.Infrastructure.TimeZoneDb.Features.SaveTimeZoneDbApiKey;

/// <summary>
/// Валидатор токена
/// </summary>
public sealed class SaveTimeZoneDbApiKeyValidator
    : AbstractValidator<SaveTimeZoneDbApiKeyUseCase>,
        IValidator<SaveTimeZoneDbApiKeyUseCase>
{
    public SaveTimeZoneDbApiKeyValidator()
    {
        AddValidationRule(
            useCase => TimeZoneDbToken.Create(useCase.ApiKey).IsSuccess,
            "Некорректный ключ Time Zone Db Api Key"
        );
    }
}
